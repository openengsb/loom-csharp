====
    Licensed to the Austrian Association for Software Tool Integration (AASTI)
    under one or more contributor license agreements. See the NOTICE file
    distributed with this work for additional information regarding copyright
    ownership. The AASTI licenses this file to you under the Apache License,
    Version 2.0 (the "License"); you may not use this file except in compliance
    with the License. You may obtain a copy of the License at

        http://www.apache.org/licenses/LICENSE-2.0

    Unless required by applicable law or agreed to in writing, software
    distributed under the License is distributed on an "AS IS" BASIS,
    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
    See the License for the specific language governing permissions and
    limitations under the License.
====


How to use the lib with the monitor
=================================================================

First of all, you need VS 2010 with C# or any equal IDE supporting .NET 4.0

In the src/ folder, you can see a seperate MockupMonitor/ and the .net-lib (lib/).
For now, the lib cannot be started without the Monitor allready running. When you start the Monitor,
a WCF SOAP-Host is gets initialized inside the monitor. So the communication is SOAP-based, which means, 
the monitor can also run on another computer then the .net-lib, herefore you have to use a custom config-file.

Before the monitor can be started in Windows 7 or Windows Vista please make sure that you've read
http://blogs.msdn.com/b/amitlale/archive/2007/01/29/addressaccessdeniedexception-cause-and-solution.aspx.
You need to register at least 8033 and 8034; also adding 35 and 36 could help for further experiments too.
For my local Windows 7 installation the following command worked like a charm in the sudo console (make
sure you adapt the command to your needs first)

    C:\Windows\system32>netsh.exe http add urlacl url=http://+:8033/ user=\pieber
    C:\Windows\system32>netsh.exe http add urlacl url=http://+:8034/ user=\pieber

In the lib, there is a simple console-application (TestConsole) with the exampleDomain to show, 
how the .net-lib can be used. 

After both, monitor and .net-lib are running, you should see in the monitor a list of methods, 
which were registered in the library or retrieved from it. You can doubleclick the registered methods and
a new window pops up, where you can insert the parameters and hit execute. The result will be shown at the bottom.
All complex types are serialized to xml and you also have to provide the xml as parameter if it's not a simple type,
or an enum. Enums are displayed using a ComboBox.
If a proxy has been retrieved and is used, the same dialog pops up, but this time, the parameter-fields are filled
and you have to fill out the return value. This time, the return value has to be a serialized object, 
even if it's a string.

Xml-Serialized c# Object:
    <Classname>
        <SimpleStringPropertyName>Hello World!</SimpleStringPropertyName>
        <SimpleIntProperty>42</SimpleIntProperty>
        <LogEnum>DEBUG</LogEnum>
    </Classname>

After you start the client you first retrieve an event from the client. For a simple response enter:

    <string>response</string>

...and press return. The string will be shown on the client console afterwards. Now we can call the method:

    Registered - Client #1, ID 0: System.String DoSomething(System.String arg0)

Therefore execute a double click in the monitor on this method and enter:

    <string>Das ist ein test</string>

...which should return:

    <?xml version="1.0" encoding="utf-16"?>
    <string>Argument retrieved: Das ist ein test</string>.

Feel free to experiment with the currently quite young state of the code base. Best to start by creating some
own interfaces and applications experimenting with the .net test client.

Possible mistakes: 
* if you want to use a custom assembly, you also have to copy it to the bin/ directory of the monitor
* some system-configurations require the program (or visual studio if you start it in debug-mode) 
  to be explicitly started as administrator
* if you create a custom application, you have to provide the config file for the lib, 
  so it can connect to the monitor. This can be done easyly, 
  by copying one of the app.config (should be equal) to the project of the new application.
* to avoid Timeouts during debuging, you should set all timeout parameters to >30min in the app.config


Build Requirements
-----------------------------------------------------------------

To build and package the projects you need a Apache Ant
(For help on installing ant see http://ant.apache.org/manual/index.html)

IMPORTANT: The build targets need MSBuild on the Path, the easiest way to accomplish this is to run Ant from a
           Visual Studio Command Prompt.
	

How to use RealDomainService:
-----------------------------------------------------------------

The library registers connectors for available domains at bus. A proxy is created automaticall
on the bus side, so any service calls to the proxy will be forwarded to the connector instance
using jms and json.

1. Compile and Package the entire solution
   enter `ant package` at the console, which generates RealDomainService.zip

2. Include all .dll in your project. All assemblies must also be deployed with your application.

3. Look into the project "ServiceTestConsole" to examine an example how to use the library.

Note: If you want to use an Mockup, see the section below. Further, the file conf/mockup.provider must be in 
your output directory. The library automatically uses the mockup.


How to use ServiceManager:
-----------------------------------------------------------------

The ServiceManager registers a set of connectors at the bus and can be configured by files.

1. Compile and Package the entire solution
   enter `ant package` at the console, which generates ServiceManager.zip

2. The ServiceManager can only be configured by config files in the `conf` directory.
   For each `.conf` file, a Service will be registered at the bus. See `example.conf` for an example.

