FruitVegScrollBackground

- Canvas-based, fixed overlay background that animates fruits/veggies with scroll progress and direction.
- Deterministic layout (seeded).
- Prefers-reduced-motion honored.

Usage:

<FruitVegScrollBackground
  sprites={[
    "/img/sprites/strawberry.png",
    "/img/sprites/kiwi.png",
    "/img/sprites/pepper.png",
    "/img/sprites/cherry.png",
    "/img/sprites/broccoli.png",
    "/img/sprites/tomato.png",
  ]}
  count={56}
  zIndex={-1}
  seed={42}
/>
