# ds4-win-plug-and-play
AbandonWare.

I'm seeing that the code is partyally in spanish, catalan and english... omg. Also is extremly dirty. It is from some time ago, sorry about that.

This program was made to solve a problem that some Windows version had with the Dual Shock 4 controller.

To use it on this OS you have to use the program InputMapper or DS4Windows.

So what the program really does is to "avoid" that some programs get the exclusive use of the controller when they are already open instead, this mode has to be for the InputMapper or DS4Windows. One of this programs is the "explorer.exe", that is the resposible of showing the windows, desktop, and task bars in Windows. So when ytou click on "Empezar" the program will kill all this programs, open the controller program, check that the controller is properly conected and open again explorer.exe.

NOTE: If you really want to use this, I recommend you to check the function "mandoConectat()", there is a hardcoded PID and VID that were the values that I got from my controller, I checked with another controller and it got the same, but it was a long time ago and maybe it is modified now or it is different from different computers.

