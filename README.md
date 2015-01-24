# WinRemoteMouse

This repository contains a bluetooth server (the console application) and two bluetooth clients (the Universal shared project, Win Store App and Win Phone 8 App).

## Overview

I wrote this code just for fun and learning purposes.

The WinRemoteMouse's server uses [32feet.NET](https://32feet.codeplex.com/) to expose a [RFCOMM](http://en.wikipedia.org/wiki/List_of_Bluetooth_protocols#Radio_frequency_communication_.28RFCOMM.29) service and makes a polling for new messages. I chose this library in order to use the server in Win7, Win8 and WinRT

The WinRemoteMouse's client app uses the [Windows.Devices.Bluetooth.Rfcomm](http://msdn.microsoft.com/en-us/library/windows/apps/xaml/windows.devices.bluetooth.rfcomm.aspx) namespace and the code is based in [this code sample](https://code.msdn.microsoft.com/windowsapps/Bluetooth-Rfcomm-Chat-afcee559)


##Usage

VS Solution have multiple startup projects.

Change the Remote Machine (Windows Store App properties) to debug the client in your Windows 8 device

Pair and connect bluetooth devices.

Run the solution...

Enable the "Win 8 remote mouse" on client.


#Roadmap

Implement something like [this one](http://twistedoakstudios.com/blog/Post3138_mouse-path-smoothing-for-jack-lumber) or use [this info](http://donewmouseaccel.blogspot.com/2009/06/out-of-sync-and-upside-down-windows.html) in order to improve the user experience

Remove left and right buttons and work only with "gestures"