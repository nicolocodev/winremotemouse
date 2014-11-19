# WinRemoteMouse

This repository contains a bluetooth server (the console application) and a bluetooth client (the Windows Store App). Windows Phone App doesn't work at this time

## Overview

I wrote this code just for fun and learning purposes.

The WinRemoteMouse's server uses [32feet.NET](https://32feet.codeplex.com/) to expose a [RFCOMM](http://en.wikipedia.org/wiki/List_of_Bluetooth_protocols#Radio_frequency_communication_.28RFCOMM.29) service (as I understand :P) and makes a polling for new messages. I chose this library in order to use the server in Win7, Win8 and WinRT

The WinRemoteMouse's client app uses the [Windows.Devices.Bluetooth.Rfcomm](http://msdn.microsoft.com/en-us/library/windows/apps/xaml/windows.devices.bluetooth.rfcomm.aspx) namespace and the code is based in [this code sample](https://code.msdn.microsoft.com/windowsapps/Bluetooth-Rfcomm-Chat-afcee559)


##Usage

VS Solution have multiple startup projects.

Change the Remote Machine (Windows Store App properties)

Pair and connect bluetooth devices.

Run the solution...

Enable the "Win 8 remote mouse" on client.

#Known issues

Coordinates maps 1:1

Code is ugly :P

#Roadmap

[Use Rx to remove the 1:1 coordinates](http://stackoverflow.com/questions/26919761/compare-mousemove-event-stream-mouse-location-doesnt-decrease) (the client code have this code commented.)

Remove left and right buttons and work only with "gestures"

Start the Windows Phone App