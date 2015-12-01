RTI README

This code will help a customer decode data collected
by a Rowe Technology Inc ADCP.  It also includes 
screening techniques to clean the ADCP data of bad data.


License
------------------------
This code is released under FreeBSD.  This license should not cause any restrictions for most users.


Getting Started
------------------------
Create a AdcpSerialPort object and subscribe to receive ReceiveAdcpSerialDataEvent events.
Create a AdcpBinaryCodec object and pass the data from the AdcpSerialPort event to the AddIncomingData().
Subscribe to AdcpBinaryCodec.ProcessDataEvent event.  This data will contain an Ensemble object with all the data decoded.
You can then use the Ensemble object to display the data.  
You can also serialize the data to JSON format using the command: Newtonsoft.Json.JsonConvert.SerializeObject(ensemble).

If you are using the project file, use AdcpDatabaseReader and AdcpDatabaseWriter.


Build
------------------------
Add the dependencies to the Properties of the project.  Then build the
project.  A DLL will be created.  I used Visual Studio 2010.

I have stripped down the DotSpatial library to a single DLL
of just the Positioning.  I had the DotSpatial project fix some bugs
i found in there code.  But since it is LGPL v2 i am not sure if
i can include the DotSpatial code in this project.  All my modifications
were passed to DotSpatial.  I just removed files to make it a smaller
package.  When you download DotSpatial, make sure you download a version
not released yet.  See below for which version.

Log4net has an Apache license and System.Data.SQLite is public domain.


Dependencies
------------------------
DotSpatial
 - At least Change Set 8d48e5c691b5 (Dec 22, 2011) or greater.  You can download under the Source Code tab.  There is a download link on the right.  I had them make some bug fixes.
http://dotspatial.codeplex.com/

log4net
http://apache.cyberuse.com//logging/log4net/binaries/log4net-1.2.11-bin-newkey.zip

System.Data.SQLite
http://system.data.sqlite.org/sqlite-netFx40-setup-bundle-x86-2010-1.0.77.0.exe


Version Numbers
------------------------
   Set version number in:
	 RTI\Properties\AssemblyInfo.cs

