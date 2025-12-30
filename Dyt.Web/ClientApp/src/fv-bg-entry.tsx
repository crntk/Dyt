
// Dyt.Web/ClientApp/src/fv-bg-entry.tsx
import * as React from "react";
import { createRoot } from "react-dom/client";
import FruitVegScrollBackground from "./background/FruitVegScrollBackground";
import "./styles/fruitveg-bg.css";


function mount() {
   

    const hostId = "fv-bg-root";
    let host = document.getElementById(hostId);

    if (host) {
        
        return;
    }

   
    host = document.createElement("div");
    host.id = hostId;

    // İlk çocuk olarak ekle - ana içeriğin arkasında kalması için
    const first = document.body.firstChild;
    if (first) {
        document.body.insertBefore(host, first);
    } else {
        document.body.appendChild(host);
    }

    host.style.position = "fixed";
    host.style.inset = "0";
    host.style.zIndex = "-1";
    host.style.pointerEvents = "none";

    

  
        createRoot(host).render(
            <FruitVegScrollBackground
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
                count={120}
                zIndex={-1}
                sizeScale={0.65}
                seed={42}
            />
        );
       
}

mount();
