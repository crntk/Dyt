import { useEffect, useRef, useState } from "react";

/**
 * Computes scroll progress (0..1) and a smoothed scroll velocity (px/sec).
 * - Uses passive listeners
 * - Throttles DOM reads to animation frames
 * - Observes resize and content changes via ResizeObserver
 */
export function useScrollProgress() {
  const [progress, setProgress] = useState(0);
  const [velocity, setVelocity] = useState(0);

  const sizesRef = useRef({ vw: 0, vh: 0, doc: 0 });
  const lastYRef = useRef(0);
  const lastTRef = useRef(performance.now());
  const rafRef = useRef<number | null>(null);
  const pendingRef = useRef(false);

  useEffect(() => {
    const updateSizes = () => {
      const vh = window.innerHeight;
      const vw = window.innerWidth;
      const doc = Math.max(
        document.body.scrollHeight,
        document.documentElement.scrollHeight,
        document.body.offsetHeight,
        document.documentElement.offsetHeight,
        document.body.clientHeight,
        document.documentElement.clientHeight
      );
      sizesRef.current = { vw, vh, doc };
    };

    updateSizes();

    const ro = new ResizeObserver(() => {
      updateSizes();
      schedule();
    });
    ro.observe(document.documentElement);

    const onScroll = () => schedule();
    const onResize = () => { updateSizes(); schedule(); };

    window.addEventListener("scroll", onScroll, { passive: true });
    window.addEventListener("resize", onResize, { passive: true });

    function schedule() {
      if (pendingRef.current) return;
      pendingRef.current = true;
      rafRef.current = requestAnimationFrame(() => {
        pendingRef.current = false;
        const now = performance.now();
        const y = window.scrollY || window.pageYOffset || 0;
        const { vh, doc } = sizesRef.current;
        const denom = Math.max(1, doc - vh);
        const p = Math.min(1, Math.max(0, y / denom));
        const dt = Math.max(1, now - lastTRef.current) / 1000;
        const vy = (y - lastYRef.current) / dt; // px/sec
        // simple smoothing
        const smoothed = 0.85 * velocity + 0.15 * vy;
        setVelocity(smoothed);
        setProgress(p);
        lastYRef.current = y;
        lastTRef.current = now;
      });
    }

    schedule();

    return () => {
      window.removeEventListener("scroll", onScroll);
      window.removeEventListener("resize", onResize);
      ro.disconnect();
      if (rafRef.current) cancelAnimationFrame(rafRef.current);
    };
  }, []);

  return { progress, velocity };
}
