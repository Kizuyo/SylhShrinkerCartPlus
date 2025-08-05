# Sylh Shrinker Cart Plus 
*(inspired by the mod ShrinkerCart by Oksamie ^^)*

**A mod for R.E.P.O. that dynamically shrinks objects inside carts.**

## ⚠️ Disclaimer
> This mod is provided **as-is** and is currently under development.  
> It was originally made for a small group of friends, so functionality may not be fully polished.  
> Some objects might behave unexpectedly or not shrink perfectly depending on their setup.

## Feedback and Suggestions
If you have any feedback or suggestions, feel free to open an issue on the [GitHub](https://github.com/Voydahn/SylhShrinkerCartPlus.git) repository.

## Mod dependancies : 
[BepInExPack v5.4.2100](https://thunderstore.io/c/repo/p/BepInEx/BepInExPack/)

## Mod Config !:
Make sure to use the '[REPOConfig](https://thunderstore.io/c/repo/p/nickklmao/REPOConfig/)' mod so you can tweak the mod’s settings directly in-game!


## 💡 Key Features
- ✅ **Smooth scaling**: no instant size changes – everything shrinks gently.
- ✅ **Safe limits**: avoids making large objects too small.
- ✅ **Restoration**: once an object leaves the cart, it gradually returns to its original size.
- ✅ **Configuration**: You can change multiple things !
- ✅ **Only Valuable**: Only valuable will be shrunk, so you can still use C.A.R.T Cannon & Laser.
- ✅ **Also change the mass**: You can activate an option to adjust the mass of shrunk objects inside the cart!
- ✅ **Infinite battery**: You can make melee, gun, drone, or cart weapons have infinite battery while in the cart.
- ✅ **Enemy instakill**: Enemies placed inside a C.A.R.T can be auto-killed (properly, with orb drop!).
- ✅ **Unbreakable Valuables**: Option to make Valuables invincible inside the cart – and even after, if you want them to stay safe forever.

Enjoy smoother carts and cleaner gameplay. No more massive furniture bouncing out like a possessed fridge. 😉

## The Wiki !
Check out the [wiki](https://thunderstore.io/c/repo/p/Sylhaance/SylhShrinkerCartPlus/wiki/) below to dive deeper into it!

# ChangeLog
## v0.4.1
- Fix the issue where objects often break when placed in the CART (sorry about that).

## v0.4.0
- MASSIVE refactor of the mod (YES! Again XD)
- Improved mod behavior in multiplayer
- Improved how an object behaves when moving from one cart to another
- Overall improvement of a bunch of stuff

## v0.3.0
- Big refactor of the mod
- Fixed a bug that prevented the Money Bag from shrinking (ups XD)
- Added a trigger system to reduce the number of lists used in the mod
- Integrated triggers into some configuration options to improve overall usability
- Made it possible to remove the "Infinite Battery" effect from items placed in a CART that originally had this option enabled

## v0.2.0
- Improved the shrink mechanism by basing it on the object's x, y, z dimensions
- Improved object resizing behavior when transferring from one CART to another

## v0.1.0
- 🚀 **Valuable safety options**:
  - Added `Valuable Safe Inside C.A.R.T`: prevent valuables from being destroyed while inside a cart.
  - Added `Valuable Stay Safe Outside C.A.R.T`: keep them unbreakable even after being removed.
- 🚀 **Battery life options**:
  - Added infinite battery for C.A.R.T weapons, melee, gun, and drone items.
- 🚀 **Enemy execution**:
  - Enemies inside a C.A.R.T can now be instantly killed (with proper orb drop).
- 🧠 **Code refactor**:
  - Smarter state tracking and cleanup for cart contents.
  - Centralized shrink logic with better handling of multiple carts.

## v0.0.10
- 🚀 Added a config option to adjust the mass of shrunk objects inside the cart

## v0.0.9
- 🐛 Fixed a bug preventing objects from being reduced when more than one CART exists in a scene

## v0.0.8
- 🚀 Added configurable shrink values for Enemy Valuable types: Tiny, Medium, and Big
- 🚀 Added a configuration option to keep shrink vualbes when they are removed from the CART

## v0.0.7
- 🚀 Added a configuration menu to customize mod settings and fine-tune your experience.

## v0.0.6
- 🐛 Fixed a bug preventing objects from returning to their original size after being removed from the cart.
- 🚀 Improved performance by shrinking only new or currently shrinking objects, instead of iterating over the entire cart content every frame.
## v0.0.6
- 🐛 Fixed a bug preventing objects from returning to their original size after being removed from the cart.
- 🚀 Improved performance by shrinking only new or currently shrinking objects, instead of iterating over the entire cart content every frame.
