--█████╗-███╗---██╗██╗███╗---███╗-█████╗-████████╗██╗-██████╗-███╗---██╗-
-██╔══██╗████╗--██║██║████╗-████║██╔══██╗╚══██╔══╝██║██╔═══██╗████╗--██║-
-███████║██╔██╗-██║██║██╔████╔██║███████║---██║---██║██║---██║██╔██╗-██║-
-██╔══██║██║╚██╗██║██║██║╚██╔╝██║██╔══██║---██║---██║██║---██║██║╚██╗██║-
-██║--██║██║-╚████║██║██║-╚═╝-██║██║--██║---██║---██║╚██████╔╝██║-╚████║-
-╚═╝--╚═╝╚═╝--╚═══╝╚═╝╚═╝-----╚═╝╚═╝--╚═╝---╚═╝---╚═╝-╚═════╝-╚═╝--╚═══╝-
-------------------------------------------------------------------------
---------████████╗███████╗███████╗████████╗███████╗██████╗---------------
---------╚══██╔══╝██╔════╝██╔════╝╚══██╔══╝██╔════╝██╔══██╗--------------
------------██║---█████╗--███████╗---██║---█████╗--██████╔╝--------------
------------██║---██╔══╝--╚════██║---██║---██╔══╝--██╔══██╗--------------
------------██║---███████╗███████║---██║---███████╗██║--██║--------------
------------╚═╝---╚══════╝╚══════╝---╚═╝---╚══════╝╚═╝--╚═╝--------------


Hi! I'm Christian, the author of AnimationTester. Thank you for your purchase!
If you're looking at this in the Unity inspector and are worried about the "noise",
I just like ASCII titles :P But I included a plaintext version of all sub-headings
just in case! :)


  ┬ ┬┬ ┬┌─┐┌┬┐  ┬┌─┐  ╔═╗╔╗╔╦╔╦╗╔═╗╔╦╗╦╔═╗╔╗╔╔╦╗╔═╗╔═╗╔╦╗╔═╗╦═╗┌─┐
  │││├─┤├─┤ │   │└─┐  ╠═╣║║║║║║║╠═╣ ║ ║║ ║║║║ ║ ║╣ ╚═╗ ║ ║╣ ╠╦╝ ┌┘
  └┴┘┴ ┴┴ ┴ ┴   ┴└─┘  ╩ ╩╝╚╝╩╩ ╩╩ ╩ ╩ ╩╚═╝╝╚╝ ╩ ╚═╝╚═╝ ╩ ╚═╝╩╚═ o
     WHAT IS ANIMATIONTESTER?

AnimationTester is a small extension which lets you choose and preview animations at
runtime, to eliminate the need for "controller juggling".

I was an animator during my game development master's, and I quickly learned that
testing models and animations in-engine is a major pain, as you not only need to
see them under different lighting conditions, you also have to create a new FSM or
controller to direct to your particular clip, or "play" until the clip you want to
check out is used.

AnimationTester solves that. Testing animations in-engine is a snap.


  ╦ ╦╔═╗╦ ╦  ┌┬┐┌─┐┌─┐┌─┐  ┬┌┬┐  ┬ ┬┌─┐┬─┐┬┌─┌─┐
  ╠═╣║ ║║║║   │││ │├┤ └─┐  │ │   ││││ │├┬┘├┴┐ ┌┘
  ╩ ╩╚═╝╚╩╝  ─┴┘└─┘└─┘└─┘  ┴ ┴   └┴┘└─┘┴└─┴ ┴ o
     HOW DOES IT WORK?

Using AnimationTester is quite straightforward. Its window will be dynamically
populated by all gameobjects in the scene with an Animator. After entering play mode,
you will then be able to select the animation, click play, and see it played!

This is possible because AnimationTester creates, at runtime, a temporary controller
with just your animation, effectively automating the process. And when you change
clip or go out of play mode, your gameobject will have its original animator,
no fiddling involved!

It's really simple, but if you want to see it in action you can go to
http://immortalhydra.com/stuff/animation-tester/ and see for yourself through the
magical power of GIFs!


  ┬ ┬┬ ┬┌─┐┬─┐┌─┐  ┌─┐┌─┐┌┐┌  ┬  ┌─┐┬┌┐┌┌┬┐  ┌┬┐┌─┐┬─┐┌─┐
  │││├─┤├┤ ├┬┘├┤   │  ├─┤│││  │  ├┤ ││││ ││  ││││ │├┬┘├┤
  └┴┘┴ ┴└─┘┴└─└─┘  └─┘┴ ┴┘└┘  ┴  └  ┴┘└┘─┴┘  ┴ ┴└─┘┴└─└─┘
            ╦╔╗╔╔═╗╔═╗╦═╗╔╦╗╔═╗╔╦╗╦╔═╗╔╗╔┌─┐
            ║║║║╠╣ ║ ║╠╦╝║║║╠═╣ ║ ║║ ║║║║ ┌┘
            ╩╝╚╝╚  ╚═╝╩╚═╩ ╩╩ ╩ ╩ ╩╚═╝╝╚╝ o
     WHERE CAN I FIND MORE INFORMATION?

Pretty much all you need to know is either here or on the AnimationTester page,
but you can write me if you want to!
You can reach me @hherebus on Twitter, or by writing me a quick email at
support@immortalhydra.com

ASCII titles realized with http://patorjk.com/software/taag/ ("ASCII Shadow" and
"Calvin S" styles).
Icons by Icons8 https://icons8.com/