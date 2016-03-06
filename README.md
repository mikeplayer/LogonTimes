# LogonTimes
Partial replacement for Windows 10 Family Safety
*Warning*  Do not use this in a corporate environment.
This program retrieves a list of everybody who can use the PC.  In a corporate environment that can take a very long time.

Windows 10 Family Safety can be extremely unreliable.  It might work for a couple of weeks and then just stop altogether - with no warning, error messages, or anything that might let a parent know that it has failed.

This is a partial replacement - with it you can manage a child's logon hours.

You can set how many hours per day a child can log on, and what hours are available for the child.

For example:<br>
On Saturday and Sunday the child can log on for a total of 6 hours between 8am and 9:30pm<br>
On Monday to Friday the child can log on for a total of 3.5 hours.  This can be any time between 6am and 7am or between 6pm and 10pm.

At the moment there are some extremely rough edges.  The most noticeable is that configuring the system is very slow.<br>
See the issues list for a more detailed list.


To install and use this program, navigate to the "Installers" directory and pick the version you need.
Download the two files and run the setup.exe file.
Run the configuration utility (it should be in your program list under LogonTimes Configuration) and configure the program.
Finally, start the LogonTimes service.

If you don't know which version you need (32 bit or 64 bit), the guildelines are:
- If you have more than 4Gb of memory, use the 64 bit version.
- Otherwise use the 32 bit version.

The easiest way to start the LogonTimes service is to reboot the PC.
The quickest way is to start the services desktop app (click start and type "services"), then scroll down to "Logon Times", right click and select "Start"
