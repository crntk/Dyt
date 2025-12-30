import * as React from "react";
import { createRng, randRange } from "./random";
import { FruitVegScrollBackgroundProps, LoadedSprite } from "./types";
import "../styles/fruitveg-bg.css";

// Sprite loader
function useSprites(urls: string[]) {
    const [sprites, setSprites] = React.useState<LoadedSprite[] | null>(null);
    React.useEffect(() => {
        if (!urls || urls.length === 0) { setSprites([]); return; }
        let cancelled = false;
        Promise.all(
            urls.map(src => new Promise<LoadedSprite>((resolve) => {
                const img = new Image();
                img.onload = () => resolve({
                    src, img,
                    w: img.naturalWidth || 64,
                    h: img.naturalHeight || 64,
                    ratio: (img.naturalWidth || 64) / (img.naturalHeight || 64)
                });
                img.onerror = () => resolve({ src, img, w: 64, h: 64, ratio: 1 });
                img.src = src;
            }))
        ).then(list => { if (!cancelled) setSprites(list); });
        return () => { cancelled = true; };
    }, [urls]);
    return sprites;
}

interface Fruit {
    x: number;
    y: number;
    vx: number;
    vy: number;
    radius: number;
    size: number;
    rotation: number;
    spriteIndex: number;
    mass: number;
    // Bireysel hareket özellikleri
    horizontalBias: number;
    verticalBias: number;
    chaosLevel: number;
    independenceTimer: number;
    isResting: boolean; // Yerde dinlenme durumu
}

// KRÝTÝK FÝX: Collision solver'a isScrollingUp parametresi eklendi
function solveFruitCollisions(fruits: Fruit[], vw: number, floorY: number, ceilingY: number, allowResting: boolean, isScrollingUp: boolean): void {
    for (let iter = 0; iter < 3; iter++) {
        // Meyve-meyve çarpýþmalarý
        for (let i = 0; i < fruits.length; i++) {
            const a = fruits[i];
            
            for (let j = i + 1; j < fruits.length; j++) {
                const b = fruits[j];
                
                // HER ÝKÝSÝ DE dinleniyorsa collision YAPMA (iç içe girmez, titremez)
                if (a.isResting && b.isResting) continue;
                
                const dx = b.x - a.x;
                const dy = b.y - a.y;
                const distSq = dx * dx + dy * dy;
                const minDist = a.radius + b.radius + 8;
                
                if (distSq < minDist * minDist && distSq > 0.01) {
                    const dist = Math.sqrt(distSq);
                    const overlap = minDist - dist;
                    const nx = dx / dist;
                    const ny = dy / dist;
                    
                    const pushStrength = overlap * 0.4;
                    
                    // EN AZ BÝRÝ hareket ediyorsa - collision uygula
                    if (!a.isResting) {
                        a.x -= nx * pushStrength;
                        a.y -= ny * pushStrength;
                    }
                    if (!b.isResting) {
                        b.x += nx * pushStrength;
                        b.y += ny * pushStrength;
                    }
                }
            }
        }
        
        // Duvarlar ve zemin/tavan
        for (const f of fruits) {
            // Dinlenen meyveler duvar kontrolü gerektirmez
            if (f.isResting) continue;
            
            // Sol duvar
            if (f.x < f.radius + 10) {
                f.x = f.radius + 10;
                if (f.vx < 0) {
                    f.vx = -f.vx * 0.25;
                }
            }
            // Sað duvar
            if (f.x > vw - f.radius - 10) {
                f.x = vw - f.radius - 10;
                if (f.vx > 0) {
                    f.vx = -f.vx * 0.25;
                }
            }
            
            // KRÝTÝK FÝX: Zemin kontrolü - Yukarý scroll sýrasýnda hýz sýfýrlanmasýn!
            const distanceToFloor = floorY - f.radius - f.y;
            if (distanceToFloor <= 2) {
                f.y = floorY - f.radius;
                
                // YUKARIDA SCROLL SRASINDA HIZ SIFIRLAMA!
                if (!isScrollingUp) {
                    if (f.vy > 5) {
                        f.vy = -f.vy * 0.05;
                    } else {
                        f.vy = 0;
                    }
                    f.vx *= 0.92;
                }
                
                if (allowResting && !isScrollingUp && Math.abs(f.vx) < 3 && Math.abs(f.vy) < 3) {
                    f.isResting = true;
                    f.vx = 0;
                    f.vy = 0;
                }
            }
            
            // Tavan
            if (f.y < ceilingY + f.radius) {
                f.y = ceilingY + f.radius;
                if (f.vy < 0) {
                    f.vy = -f.vy * 0.05;
                }
            }
        }
    }
}

export const FruitVegScrollBackground: React.FC<FruitVegScrollBackgroundProps> = ({
    sprites,
    count = 35,
    zIndex = 0,
    seed = 1,
    sizeScale = 1.25,
}) => {
    const canvasRef = React.useRef<HTMLCanvasElement | null>(null);
    const spritesLoaded = useSprites(sprites || []);
    
    const dataRef = React.useRef<{
        fruits: Fruit[];
        lastScrollY: number;
        scrollVelocity: number;
        vw: number;
        vh: number;
        initialized: boolean;
    }>({
        fruits: [],
        lastScrollY: 0,
        scrollVelocity: 0,
        vw: 0,
        vh: 0,
        initialized: false,
    });

    React.useEffect(() => {
        const canvas = canvasRef.current;
        if (!canvas) return;

        const data = dataRef.current;
        let animId = 0;
        let lastTime = 0;

        const createFruits = () => {
            const vw = window.innerWidth;
            const vh = window.innerHeight;
            const currentScrollY = window.scrollY;
            
            data.vw = vw;
            data.vh = vh;
            data.lastScrollY = currentScrollY;
            
            const rng = createRng(seed);
            const fruits: Fruit[] = [];
            
            const viewportBottom = currentScrollY + vh;
            const pageBottom = viewportBottom - 10;
            
            for (let i = 0; i < count; i++) {
                const size = randRange(rng, 50, 85) * sizeScale;
                const radius = size * 0.4;
                
                fruits.push({
                    x: randRange(rng, radius + 40, vw - radius - 40),
                    y: pageBottom - randRange(rng, 0, 80),
                    vx: 0,
                    vy: 0,
                    radius,
                    size,
                    rotation: 0,
                    spriteIndex: i % Math.max(1, sprites.length),
                    mass: radius * radius,
                    horizontalBias: (Math.random() - 0.5) * 1.2,
                    verticalBias: (Math.random() - 0.5) * 1.2,
                    chaosLevel: 0.4 + Math.random() * 0.4,
                    independenceTimer: Math.random() * 5,
                    isResting: false,
                });
            }
            
            const ceilingY = currentScrollY;
            for (let i = 0; i < 40; i++) {
                solveFruitCollisions(fruits, vw, pageBottom - 5, ceilingY, true, false);
            }
            fruits.forEach(f => {
                f.vx = 0; 
                f.vy = 0;
                f.isResting = true;
            });
            
            data.fruits = fruits;
            data.initialized = true;
        };

        const animate = (time: number) => {
            animId = requestAnimationFrame(animate);
            
            const dt = Math.min(0.02, (time - lastTime) / 1000);
            lastTime = time;
            if (dt <= 0) return;

            const vw = window.innerWidth;
            const vh = window.innerHeight;
            
            if (data.vw !== vw || data.vh !== vh || !data.initialized) {
                createFruits();
            }

            const ctx = canvas.getContext("2d");
            if (!ctx) return;

            const dpr = window.devicePixelRatio || 1;
            if (canvas.width !== vw * dpr || canvas.height !== vh * dpr) {
                canvas.width = vw * dpr;
                canvas.height = vh * dpr;
                canvas.style.width = vw + "px";
                canvas.style.height = vh + "px";
                ctx.setTransform(dpr, 0, 0, dpr, 0, 0);
            }

            const currentScrollY = window.scrollY;
            const scrollDelta = currentScrollY - data.lastScrollY;
            data.scrollVelocity = data.scrollVelocity * 0.12 + scrollDelta * 0.88;
            data.lastScrollY = currentScrollY;
            
            const docHeight = document.documentElement.scrollHeight;
            const footerHeight = 80;
            const viewportBottom = currentScrollY + vh;
            const documentBottom = docHeight - footerHeight;
            
            // Footer yakýnýnda floorY - GEÇÝÞ BÖLGE YOK
            let floorY: number;
            const distanceToDocBottom = documentBottom - viewportBottom;
            
            if (distanceToDocBottom <= 0) {
                // Footer görünür - zemin SABÝT
                floorY = documentBottom;
            } else {
                // Normal scroll - zemin viewport ile hareket eder
                floorY = viewportBottom - 10;
            }
            
            const ceilingY = currentScrollY;
            const gravity = 1000;
            const { fruits } = data;
            const scrollVel = data.scrollVelocity;
            const scrollThreshold = 0.8;
            
            const isScrolling = Math.abs(scrollVel) >= scrollThreshold;
            const isScrollingUp = scrollVel < -scrollThreshold;
            const isScrollingDown = scrollVel > scrollThreshold;

            // KRÝTÝK FÝX: Yukarý scroll baþladýðýnda TÜM meyveleri aktifleþtir
            if (isScrollingUp) {
                for (const f of fruits) {
                    f.isResting = false;
                }
            }

            for (const f of fruits) {
                f.independenceTimer += dt;
                
                const screenY = f.y - currentScrollY;
                const screenFloorY = floorY - currentScrollY;
                const distanceToFloor = screenFloorY - f.radius - screenY;
                
                if (isScrollingDown) {
                    // AÞAÐI SCROLL
                    f.isResting = false;
                    
                    f.vy += gravity * dt * 1.5;
                    f.vx += f.horizontalBias * 400 * dt * f.chaosLevel;
                    f.vx += (Math.random() - 0.5) * 800 * dt;
                    f.vy += (Math.random() - 0.3) * 400 * dt;
                    
                    if (Math.random() < 0.6) {
                        const sideForce = (Math.random() - 0.5) * 700 * f.chaosLevel;
                        f.vx += sideForce * dt;
                    }
                    
                    if (Math.random() < 0.4 * f.chaosLevel) {
                        const spinAngle = (Math.random() * Math.PI * 2) + (f.independenceTimer * 1.5);
                        const spinPower = 300 + Math.random() * 200;
                        f.vx += Math.cos(spinAngle) * spinPower * dt;
                        f.vy += Math.sin(spinAngle) * spinPower * dt * 0.4;
                    }
                    
                } else if (isScrollingUp) {
                    // YUKARI SCROLL - KRÝTÝK FÝX: Ekranda görünen meyveler için doðru algýlama
                    
                    // Viewport içinde mi kontrol et (0 ile vh arasýnda)
                    const isInViewport = screenY >= 0 && screenY <= vh;
                    const isInLowerViewport = screenY > vh * 0.2 && screenY <= vh; // Alt %80
                    
                    const absoluteDistanceToFloor = Math.abs(distanceToFloor);
                    const detectionRange = vh * 0.7; // Viewport yüksekliðinin %70'i
                    
                    const isNearFloor = absoluteDistanceToFloor <= detectionRange;
                    const isVeryCloseToFloor = absoluteDistanceToFloor <= 50;
                    
                    // Tabandaki veya alt viewport'taki meyveler için GÜÇLÜ kuvvet
                    if ((isNearFloor || isInLowerViewport) && isInViewport) {
                        let proximity: number;
                        
                        if (isVeryCloseToFloor) {
                            proximity = 1.0;
                        } else if (isInLowerViewport) {
                            proximity = 0.7 + (Math.random() * 0.15); // Azaltýldý: 0.8 ? 0.7
                        } else {
                            const normalizedDist = Math.min(1, absoluteDistanceToFloor / detectionRange);
                            proximity = 1.0 - (normalizedDist * 0.6); // Azaltýldý: 0.5 ? 0.6
                        }
                        
                        // BOOST DEÐERLERÝ %30 DAHA AZALTILDI
                        const baseBoost = 500 + Math.random() * 350; // Önceki: 700 + 500
                        const proximityMultiplier = 1.0 + (proximity * 1.2); // Önceki: 1.2 + 1.5
                        
                        f.vy -= baseBoost * proximityMultiplier * dt * 1.2; // Önceki: dt * 1.5
                        
                        const lateralBurst = (Math.random() - 0.5) * 500; // Önceki: 750
                        f.vx += lateralBurst * dt;
                        
                        if (Math.random() < 0.6) { // Önceki: 0.7
                            const explosionAngle = (Math.random() * Math.PI * 0.6) - (Math.PI * 0.8);
                            const explosionPower = 400 + Math.random() * 300; // Önceki: 600 + 450
                            f.vx += Math.cos(explosionAngle) * explosionPower * dt;
                            f.vy += Math.sin(explosionAngle) * explosionPower * dt;
                        }
                    } else {
                        // Üst viewport veya ekran dýþýndaki meyveler - normal yukarý kuvvet
                        const upwardForce = 300 + Math.random() * 200; // Önceki: 400 + 300
                        f.vy -= upwardForce * dt * 1.3; // Önceki: dt * 1.5
                    }
                    
                    // Genel yukarý kuvvet - TÜM MEYVELER ÝÇÝN
                    f.vy -= gravity * dt * 0.8; // Önceki: dt * 1.0
                    
                    f.vx += f.horizontalBias * 250 * dt * f.chaosLevel; // Önceki: 300
                    f.vy += f.verticalBias * 150 * dt * f.chaosLevel; // Önceki: 200
                    
                    const horizontalForce = (Math.random() - 0.5) * 450; // Önceki: 600
                    f.vx += horizontalForce * dt * f.chaosLevel;
                    
                    f.vy += (Math.random() - 0.85) * 180 * dt; // Önceki: 250
                    
                    if (Math.random() < 0.25 * f.chaosLevel) { // Önceki: 0.3
                        const angle = (Math.random() * Math.PI * 0.8) - (Math.PI * 0.65);
                        const burstPower = 220 + Math.random() * 180; // Önceki: 300 + 250
                        f.vx += Math.cos(angle) * burstPower * dt;
                        f.vy += Math.sin(angle) * burstPower * dt;
                    }
                    
                    if (Math.random() < 0.2) { // Önceki: 0.25
                        const turbAngle = Math.random() * Math.PI * 2;
                        const turbulence = 130 + Math.random() * 130; // Önceki: 180 + 180
                        f.vx += Math.cos(turbAngle) * turbulence * dt;
                        f.vy += Math.sin(turbAngle) * turbulence * dt * 0.4;
                    }
                    
                } else {
                    // SCROLL DURDU
                    if (f.isResting) {
                        // TAM DURGUN - HÝÇBÝR ÝÞLEM YAPMA
                        f.vx = 0;
                        f.vy = 0;
                        continue; // Bir sonraki meyveye geç
                    } else {
                        if (distanceToFloor <= 5) {
                            const speedThreshold = 5; // Daha düþük eþik - daha hýzlý durma
    
                            if (Math.abs(f.vx) < speedThreshold && Math.abs(f.vy) < speedThreshold) {
                                f.vx = 0;
                                f.vy = 0;
                                f.isResting = true;
                            } else {
                                f.vx *= 0.85; // Daha hýzlý yavaþlama
                                f.vy *= 0.85;
                            }
                        } else {
                            f.vy += gravity * dt * 0.6;
                            f.vx *= 0.994;
                            f.vy *= 0.996;
                        }
                    }
                }
            }
            
            // Pozisyon güncellemesi
            for (const f of fruits) {
                // Dinlenen meyveleri güncelleme
                if (f.isResting) {
                    f.vx = 0;
                    f.vy = 0;
                    continue; // Pozisyon güncellemesi yok
                }
                
                if (isScrolling) {
                    f.x += f.vx * dt;
                    f.y += f.vy * dt;
                    f.vx *= 0.992;
                    f.vy *= 0.996;
                } else {
                    f.x += f.vx * dt;
                    f.y += f.vy * dt;
                }
            }

            // KRÝTÝK FÝX: Collision solver'a isScrollingUp parametresi geçiliyor
            solveFruitCollisions(fruits, vw, floorY, ceilingY, !isScrolling, isScrollingUp);

            // ÇÝZÝM
            ctx.clearRect(0, 0, vw, vh);
            
            const images = spritesLoaded;
            const sorted = [...fruits].sort((a, b) => a.y - b.y);
            
            for (const f of sorted) {
                const screenY = f.y - currentScrollY;
                
                if (screenY < -f.size || screenY > vh + f.size) continue;
                
                ctx.globalAlpha = 0.95;
                
                if (images && images.length > 0) {
                    const sprite = images[f.spriteIndex % images.length];
                    const w = f.size;
                    const h = f.size / sprite.ratio;
                    
                    ctx.save();
                    ctx.translate(f.x, screenY);
                    ctx.drawImage(sprite.img, -w/2, -h/2, w, h);
                    ctx.restore();
                } else {
                    ctx.beginPath();
                    ctx.arc(f.x, screenY, f.radius, 0, Math.PI * 2);
                    ctx.fillStyle = `hsl(${f.spriteIndex * 40}, 70%, 55%)`;
                    ctx.fill();
                }
            }
            
            ctx.globalAlpha = 1;
        };

        createFruits();
        animId = requestAnimationFrame(animate);
        
        const onResize = () => { data.initialized = false; };
        window.addEventListener("resize", onResize);
        
        return () => {
            cancelAnimationFrame(animId);
            window.removeEventListener("resize", onResize);
        };
    }, [spritesLoaded, count, seed, sizeScale, sprites.length]);

    return (
        <canvas
            ref={canvasRef}
            className="fv-bg"
            style={{ ["--z" as any]: String(zIndex ?? -1) }}
        />
    );
};

export default FruitVegScrollBackground;
