import * as React from "react";
import { createRoot } from "react-dom/client";
import BowlSpillAnimation from "./background/BowlSpillAnimation";
import "./styles/bowl-spill.css";

function mountBowlSpill() {
    // Sadece ana sayfada çalışsın
    if (window.location.pathname !== "/") return;

    // Daha önce gösterildi mi kontrol et
    const hasSeenAnimation = sessionStorage.getItem("bowlAnimationShown");
    if (hasSeenAnimation) return;

    const hostId = "bowl-spill-root";
    let host = document.getElementById(hostId);

    if (!host) {
        host = document.createElement("div");
        host.id = hostId;
        document.body.appendChild(host);
    }

    const handleComplete = () => {
        // Animasyon tamamlandığında session'a kaydet
        sessionStorage.setItem("bowlAnimationShown", "true");

        // Host'u kaldır
        setTimeout(() => {
            if (host && host.parentNode) {
                host.parentNode.removeChild(host);
            }
        }, 500);
    };

    createRoot(host).render(
        <BowlSpillAnimation
            sprites={[
                "/img/sprites/biber.png",
                "/img/sprites/brokoli.png",
                "/img/sprites/cilek.png",
                "/img/sprites/dolmabiber.png",
                "/img/sprites/domates.png",
                "/img/sprites/kiraz.png",
                "/img/sprites/kivi.png",
                "/img/sprites/muz.png",
                "/img/sprites/portakal.png",
            ]}
            fruitCount={100}
            onAnimationComplete={handleComplete}
        />
    );
}

// Sayfa yüklendiğinde çalıştır
if (document.readyState === "loading") {
    document.addEventListener("DOMContentLoaded", mountBowlSpill);
} else {
    mountBowlSpill();
}