Requirements
=================
Visual studio 2012
or
Visual studio and nuget

Build
=================
Use visual studio to build the project. However, to do it over the command prompt, use the following command

msbuild bridge/bridge.sln

Tests
=================
Use Visual studio to execute the tests cases. To execute the tests from the command prompt, use the following command

mstest /testmetadata:Testsettings.vsmdi

Summary
=================
While opening the loom-csharp\bridge\Bridge.sln, Visual studio automatically restores the nuget packages.

The documentation can be found under the following link.
http://requirements.openengsb.org/confluence/display/MANUAL/.Net+Bridge
