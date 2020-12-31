# pi-top-samples
samples for .net core development on the pi-top

## prerequisites
To run these samples, you need a pi-top (https://www.pi-top.com) and for most  samples also their foundation kit.

The samples assume that you have set up the pi-top, and can connect to it over ssh.

I'm using the (sshdeploy)[https://github.com/unosquare/sshdeploy] tool to deploy code to the pi. 

The workflow to push code to the pi-top is then:

```
dotnet build -c release
dotnet sshdeploy push -w <pi-top password>
```

To run the sample, connect to your pi-top via ssh (`ssh pi@pi-top`), and run the following commands (example for the helloworld sample; similar for the others):

```
cd helloworld
dotnet helloworld.dll
```
