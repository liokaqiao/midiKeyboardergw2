# midiKeyboardergw2
play music in guildwars 2 with your midi keyboard
Building:  
requires InputManager

http://www.codeproject.com/Articles/117657/InputManager-library-Track-user-input-and-simulate

Midi .net
https://code.google.com/p/midi-dot-net/

Getting it:
If you don't have app building ability click the get zip button to the right.  There will be a folder in there "release" which has the app pre-built.

How to play:  Setup:

1.  Install your midi keyboard and drivers for it.
2.  Connect the keyboard and power it up.
3.  Run the app compiled from this program.
4.  Select the input device and output device (so you can hear what is heard ingame without lag).  Microsoft wavesynth will have lag in it so its best to use your midi keyboard output if it exists.
5.  Check the connect box next to the input and output devices.
6. Start playing!


Playing:

1.  Launch Guildwars 2.  Log in, grab an instrument (harp works best).
2.  Start the instrument, then start playing on your keyboard.  You should hear the same sounds in game.

Other features:
Numeric spinner with the 30:  used to tune the delay between virtual keyboard presses, now does nothing.

The dropdown with notes:  Select the key you're playing in.  This transposes your key into the range of C or A minor. 
  helps to turn down your TV or pc speakers when playing off C
  
  
1034/980:  used to be for the octave manager but turns out anet's animated icons are too slow for it.

Flute:  Check if playing the flute (changes the octave manager)  Flute plays in range from C4 to C6
Bass:  check if playing the bass.  The bass has only 2 octaves.
Flat:  Check if off C and playing a key with flats in it.  This shifts the flats up.  Uncheck for keys with Sharps.

Limitations and notes:
There is currently a limitation where Guildwars 2 does not have the Black keys on instruments.  This prevents key changes and playing naturally outside of C or A minor.  Keys F and G may be faked to a degree depending on the song:  Check the Flat to play F# as G, uncheck it to play Bb as A.  This turns Bb chord into Dm, and D chord to Dsus4.  In G key i recommend playing D5 instead if you want the D chord as this does not use the F#.

The transposer works best with D, E, and B keys.  It does not support half tone keys (like Eb)
