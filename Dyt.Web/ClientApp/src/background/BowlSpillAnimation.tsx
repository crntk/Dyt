import * as React from "react";
import { createRng, randRange } from "./random.js";
import "../styles/bowl-spill.css";

interface LoadedSprite {
    src: string;
    img: HTMLImageElement;
    w: number;
    h: number;
    ratio: number;
}

interface BowlSpillAnimationProps {
    sprites: string[];
    fruitCount?: number;
    onAnimationComplete?: () => void;
}

interface Fruit {
    x: number;
    y: number;
    vx: number;
    vy: number;
    rotation: number;
    rotationSpeed: number;
    size: number;
    spriteIndex: number;
    delay: number;
    hasStarted: boolean;
}

// Sprite yükleme hook'u
function useSprites(urls: string[]) {
    const [sprites, setSprites] = React.useState<LoadedSprite[] | null>(null);

    React.useEffect(() => {
        if (!urls || urls.length === 0) {
            setSprites([]);
            return;
        }

        let cancelled = false;
        Promise.all(
            urls.map(src => new Promise<LoadedSprite>((resolve) => {
                const img = new Image();
                img.onload = () => resolve({
                    src,
                    img,
                    w: img.naturalWidth || 64,
                    h: img.naturalHeight || 64,
                    ratio: (img.naturalWidth || 64) / (img.naturalHeight || 64)
                });
                img.onerror = () => resolve({ src, img, w: 64, h: 64, ratio: 1 });
                img.src = src;
            }))
        ).then(list => {
            if (!cancelled) setSprites(list);
        });

        return () => { cancelled = true; };
    }, [urls]);

    return sprites;
}

export const BowlSpillAnimation: React.FC<BowlSpillAnimationProps> = ({
    sprites,
    fruitCount = 61,
    onAnimationComplete
}) => {
    const canvasRef = React.useRef<HTMLCanvasElement | null>(null);
    const bowlRef = React.useRef<HTMLDivElement | null>(null);
    const spritesLoaded = useSprites(sprites || []);
    const [isAnimating, setIsAnimating] = React.useState(true);

    const dataRef = React.useRef<{
        fruits: Fruit[];
        animationStartTime: number;
        initialized: boolean;
    }>({
        fruits: [],
        animationStartTime: 0,
        initialized: false
    });

    React.useEffect(() => {
        const canvas = canvasRef.current;
        if (!canvas || !spritesLoaded || spritesLoaded.length === 0) return;

        const data = dataRef.current;
        let animId = 0;
        let lastTime = 0;

        const createFruits = () => {
 const vw = window.innerWidth;
            const vh = window.innerHeight;

    // Kase pozisyonu - Biraz daha yukarıda
   const bowlX = vw / 2;
  const bowlY = vh * 0.45;
            const bowlRadius = 140;

            const rng = createRng(Date.now());
   const fruits: Fruit[] = [];

  // Meyveler kasede başlar
     for (let i = 0; i < fruitCount; i++) {
            const angle = (Math.PI * 2 * i) / fruitCount;
          const radius = randRange(rng, 10, bowlRadius - 40);
    const size = randRange(rng, 40, 70);

                fruits.push({
     x: bowlX + Math.cos(angle) * radius,
      y: bowlY + Math.sin(angle) * radius * 0.4,
    vx: (Math.random() - 0.5) * 400 + Math.cos(angle) * 200,
     vy: -Math.random() * 600 - 400,
   rotation: Math.random() * Math.PI * 2,
 rotationSpeed: (Math.random() - 0.5) * 10,
          size,
      spriteIndex: i % sprites.length,
       delay: i * 25,
             hasStarted: false
        });
       }

            data.fruits = fruits;
            data.animationStartTime = performance.now();
       data.initialized = true;
        };

        const animate = (time: number) => {
            animId = requestAnimationFrame(animate);

            const dt = Math.min(0.02, (time - lastTime) / 1000);
            lastTime = time;
            if (dt <= 0) return;

            const vw = window.innerWidth;
            const vh = window.innerHeight;

            if (!data.initialized) {
                createFruits();
            }

            const ctx = canvas.getContext("2d");
            if (!ctx) return;

            // Canvas boyutlandırma
            const dpr = window.devicePixelRatio || 1;
            if (canvas.width !== vw * dpr || canvas.height !== vh * dpr) {
                canvas.width = vw * dpr;
                canvas.height = vh * dpr;
                canvas.style.width = vw + "px";
                canvas.style.height = vh + "px";
                ctx.setTransform(dpr, 0, 0, dpr, 0, 0);
            }

            const elapsed = time - data.animationStartTime;
            const { fruits } = data;
            const gravity = 900;
            const floorY = vh + 100;

            let allStopped = true;

            for (const f of fruits) {
                if (elapsed < f.delay) {
                    allStopped = false;
                    continue;
                }

                if (!f.hasStarted) {
                    f.hasStarted = true;
                }

                f.vy += gravity * dt;
                f.x += f.vx * dt;
                f.y += f.vy * dt;
                f.rotation += f.rotationSpeed * dt;

                f.vx *= 0.995;
                f.vy *= 0.998;

                if (f.y < floorY) {
                    allStopped = false;
                }

                if (f.x < 0 || f.x > vw) {
                    f.vx = -f.vx * 0.7;
                    f.x = Math.max(0, Math.min(vw, f.x));
                }
            }

            // Çizim
            ctx.clearRect(0, 0, vw, vh);

            for (const f of fruits) {
   if (!f.hasStarted) continue;

    ctx.save();
 ctx.translate(f.x, f.y);
       ctx.rotate(f.rotation);
        ctx.globalAlpha = 0.85; // Değiştirildi: Arka plan şeffaflığı azaltıldı (0.7'den 0.85'e)

                const sprite = spritesLoaded[f.spriteIndex % spritesLoaded.length];
            const w = f.size;
            const h = f.size / sprite.ratio;

   ctx.drawImage(sprite.img, -w / 2, -h / 2, w, h);
             ctx.restore();
            }

            if (allStopped && elapsed > 3500) {
                setIsAnimating(false);
                if (onAnimationComplete) {
                    onAnimationComplete();
                }
                cancelAnimationFrame(animId);
            }
        };

        createFruits();
        animId = requestAnimationFrame(animate);

        return () => {
            cancelAnimationFrame(animId);
        };
    }, [spritesLoaded, fruitCount, sprites.length, onAnimationComplete]);

    if (!isAnimating) return null;

    return (
        <div className="bowl-spill-container" style={{ background: 'transparent' }}>
            {/* Kase SVG */}
            <div ref={bowlRef} className="bowl">
              <svg viewBox="0 0 200 120" xmlns="http://www.w3.org/2000/svg">
   {/* Kase gölgesi */}
    <ellipse cx="100" cy="110" rx="85" ry="15" fill="rgba(0,0,0,0.1)" />

        {/* Kase */}
           <defs>
     <linearGradient id="bowlGradient" x1="0%" y1="0%" x2="0%" y2="100%">
          <stop offset="0%" style={{ stopColor: '#4CAF50', stopOpacity: 1 }} />
    <stop offset="100%" style={{ stopColor: '#FF9800', stopOpacity: 1 }} />
                    </linearGradient>
      </defs>

   {/* Kase dış kenarı */}
    <ellipse cx="100" cy="40" rx="90" ry="20" fill="url(#bowlGradient)" />

    {/* Kase gövdesi */}
<path
              d="M 10 40 Q 10 90, 100 100 Q 190 90, 190 40 Z"
                 fill="url(#bowlGradient)"
      opacity="0.9"
         />

           {/* Kase iç tarafı */}
         <ellipse cx="100" cy="40" rx="85" ry="18" fill="#81C784" />

        {/* Highlight */}
    <ellipse cx="80" cy="35" rx="30" ry="8" fill="rgba(255,255,255,0.4)" />
   </svg>
            </div>

            {/* Meyveler canvas */}
            <canvas ref={canvasRef} className="bowl-spill-canvas" />
        </div>
    );
};

export default BowlSpillAnimation;