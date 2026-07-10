# Performance (B4)

Guidance and tooling for targeting lower-end hardware. Numbers in this document are estimates
for catching problems early at design time - always confirm with the Unity Profiler / Frame
Debugger on a real device before shipping a performance claim.

## uGUI backend

- **Auto sub-canvas splitting.** `UGUISurface` gives every screen its own nested `Canvas`
  component by default (added automatically the first time a screen is opened, unless the
  screen's prefab already defines one). A nested Canvas creates its own batching boundary, so
  one screen rebuilding its geometry doesn't force Unity to rebatch sibling screens under the
  same parent overlay Canvas. It inherits render mode, `EventSystem`, and `GraphicRaycaster`
  from the parent - you don't need to add those yourself, and adding your own
  `GraphicRaycaster` on top would double-raycast input.
- **Keep animated elements last in the hierarchy.** The Profiler panel (`Tools/NexUI/Designer/UI
  Profiler`) flags any motion-bound element that isn't the topmost sibling among elements
  sharing the same parent. Use **Bring to Front** in the Designer canvas context menu to move it
  there - a uGUI Canvas rebuilds geometry for an element and everything drawn after it, so a
  static background sitting *above* an animating element in the hierarchy gets needlessly
  rebuilt every frame the animation runs.
- **List/grid pooling.** `emiteat.NexUI.Components.UIItemPool<TView>` (Runtime/Components) is a
  generic recycle pool for `INXList`/`INXGrid` item views - reuse it instead of
  instantiate/destroy per `SetItems`/`Refresh` call. It's backend-agnostic (works with a uGUI
  component type or a UI Toolkit `VisualElement`).

## UI Toolkit backend

- **Vertex Budget.** Panel Settings defaults to an unbounded (`0`) Vertex Budget. The Profiler
  panel's "recommended Vertex Budget" reading is a heuristic estimate (element-type + text-length
  based) with headroom built in - punch that number into your `PanelSettings` asset instead of
  leaving it on auto.
- **Texture batching.** uGUI's default UI shader batches up to 8 distinct textures per draw call.
  The Profiler panel's "estimated batch groups" is a *proxy* metric (distinct tint/text groups,
  since Designer element metadata doesn't currently track a per-element sprite/texture
  reference) - treat a high count as a hint to check your actual sprite atlasing, not as an
  exact draw-call count.
- **Prefer rectangular clipping over stencil masks.** UI Toolkit's `overflow: hidden` /
  `Overflow.Hidden` on a `VisualElement` clips with a cheap scissor rect. A uGUI `RectMask2D`
  is the equivalent cheap option. Reach for a full `Mask`/stencil component only when the clip
  region is genuinely non-rectangular (a circular avatar, a diagonal reveal, etc.) - stencil
  masking costs an extra draw call and breaks batching for everything inside it, so use it
  narrowly rather than as a default.

## Cross-cutting

- **Motion tick path** (`BuiltInMotionPlayer.PlayAsync`/`ApplyAt`/`ApplyProperty`): audited for
  B4 - no per-frame heap allocations (no LINQ, no per-frame `new List/Dictionary`, no per-frame
  closures). The only allocations are once per motion *start* (a linked `CancellationTokenSource`
  and a dictionary entry), which is expected and low-cost relative to a multi-second animation.
- **Command Pipeline dispatch** (`UICommandDispatcher.DispatchAsync`): previously rebuilt the
  middleware chain as N nested closures on every single dispatch. Reworked to a single reusable
  per-dispatch state object that walks the chain by index instead - cuts a dispatch with N
  middlewares from N closure allocations down to one. This only runs per user command (click/
  open/close), not per frame, so it was a secondary priority next to the Motion tick path, but
  the fix was small and self-contained.
- **Profiler panel** (`Tools/NexUI/Designer/UI Profiler`): now reports an estimated vertex count,
  a recommended Vertex Budget, a batch-group proxy, an overdraw estimate (sum of pairwise element
  rect overlap over an assumed 1920x1080 canvas), and a "largest cost contributor" classification
  (element count / overdraw / motion count / batch groups / binding count - whichever is
  furthest past its guideline threshold), alongside the previous static counts.
