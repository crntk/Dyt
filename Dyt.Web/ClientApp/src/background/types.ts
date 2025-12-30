/**
 * Shared types for Fruit & Veg background system
 */

export type GravityCurve = "soft" | "medium" | "hard";

export type FruitVegScrollBackgroundProps = {
  /** Transparent PNG/SVG URLs for fruits/veggies */
  sprites: string[];
  /** Number of items to render (default 48, clamped 1..120) */
  count?: number;
  /** z-index for fixed overlay (default 0) */
  zIndex?: number;
  /**
   * NEW: visual tuning
   */
  /** Multiplier for generated sizes (default 1.35) */
  sizeScale?: number;
  /** Amplitude of continuous drift in px (default 12) */
  floatStrength?: number;
  /** Global drift frequency in Hz (default 0.06) */
  floatFrequency?: number;
  /** Per-item random walk strength in px (default 10) */
  noiseStrength?: number;
  /** Spring factor towards selected pile (default 0.14) */
  pileAttraction?: number;
  /** Damping while settling (default 0.18) */
  settleDamping?: number;
  /** Maximum drift speed in px/sec (default 18) */
  maxSpeed?: number;
  /** Interpolation curve strength (default "medium") */
  gravityCurve?: GravityCurve;
  /** Thickness ratio for top pile band (0..0.5, default 0.18) */
  topPileHeightRatio?: number;
  /** Thickness ratio for bottom pile band (0..0.5, default 0.22) */
  bottomPileHeightRatio?: number;
  /** Seed for deterministic layout (default 1) */
  seed?: number;
};

export type LoadedSprite = {
  src: string;
  img: HTMLImageElement;
  w: number;
  h: number;
  ratio: number; // w/h
};

export type Item = {
  id: number;
  x: number; // base x in [0, vw]
  size: number; // base size in css px
  rot: number; // current rotation (rad)
  rotSpeed: number; // rad/sec
  spriteIndex: number; // index in loaded sprites
  // anchors in viewport coords
  topX: number;
  topY: number;
  bottomX: number;
  bottomY: number;
  // per-item subtle parallax factors
  parallaxX: number; // -1..1
  parallaxY: number; // -1..1
  // color for fallback blobs
  color: string;
  // runtime state (created lazily)
  xNow?: number;
  yNow?: number;
  vx?: number;
  vy?: number;
};
