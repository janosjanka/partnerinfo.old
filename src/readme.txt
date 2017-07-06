IIS EXPRESS (HTTP + HTTPS)

-------------------------------------------------------------------------------------------------------------------------------

1.) We need to tell HTTP.SYS at the kernel level that it's OK to let everyone talk to this URL by making an "Url Reservation." 

netsh http add urlacl url=http://localhost-partnerinfo.tv:80/ user=everyone
netsh http add urlacl url=https://localhost-partnerinfo.tv:443/ user=everyone

-------------------------------------------------------------------------------------------------------------------------------

2.) Next, as I want to be able to talk to IIS Express from outside (folks on my network, etc. Not just localhost)
then I need to allow IIS Express through the Windows Firewall.

netsh firewall add portopening TCP 80 IISExpressWeb enable ALL

-------------------------------------------------------------------------------------------------------------------------------

3.) Let's make a SSL certificate of our own. Note the CN=. I'm making it my Computer Name,
but you could make it nerddinner.com or whatever makes you happy.
It should line up with whatever name you've been using so far.

- "%PROGRAMFILES(X86)%\Windows Kits\8.0\bin\x64\makecert.exe" -r -pe -n "CN=localhost-partnerinfo.tv" -b 01/01/2000 -e 01/01/2100 -eku 1.3.6.1.5.5.7.3.1 -ss my -sr localMachine -sky exchange -sp "Microsoft RSA SChannel Cryptographic Provider" -sy 12

- Run MMC.exe, go File | Add/Remove Snap In, then select Certificates. Pick the Computer Account. (This is why you can't just run certmgr.msc) and add it.

- It'll likely be the certificate with an expiration data of 1/1/2100 under Personal Certificates.
Double click on your certificate. Go to Details, and scroll down to Thumbprint.
Copy that into the clipboard, as that identifies our new certificate.

- Remove all the spaces from that Thumbprint hash. You can remove those spaces with Notepad if you're Phil Haack, or in PowerShell if you're awesome:
C:\>"‎23 49 61 d7 32 17 84 ed b8 5b c3 e4 ff 4c 00 9a 95 2d b5 08" -replace " "

234961d7321784edb85bc3e4ff4c009a952db508

- Take the hash and plug it in to the end of THIS command:
netsh http add sslcert ipport=0.0.0.0:443 certhash=234961d7321784edb85bc3e4ff4c009a952db508 appid='{214124cd-d05b-4309-9af9-9caa44b2b74a}'

- Go back to the CertMgr MMC, and drag your self-signed SSL Certificate from Personal into Trusted Root Certificates.

http://www.hanselman.com/blog/WorkingWithSSLAtDevelopmentTimeIsEasierWithIISExpress.aspx

-------------------------------------------------------------------------------------------------------------------------------

3.) c:\Windows\system32\drivers\etc\hosts

127.0.0.1				localhost-partnerinfo.tv
127.0.0.1				api.localhost-partnerinfo.tv

-------------------------------------------------------------------------------------------------------------------------------

4.) IIS Express: %USERPROFILE%\My Documents\IISExpress\config\applicationhost.config

<site name="Partnerinfo.Web.Api" id="1">
<application path="/" applicationPool="Clr4IntegratedAppPool">
    <virtualDirectory path="/" physicalPath="E:\Projects v1\src\Partnerinfo.Web.Api" />
</application>
<bindings>
    <binding protocol="http" bindingInformation="*:11100:api.localhost-partnerinfo.tv" />
</bindings>
</site>
<site name="Partnerinfo.Web.App" id="2">
<application path="/" applicationPool="Clr4IntegratedAppPool">
    <virtualDirectory path="/" physicalPath="E:\Projects v1\src\Partnerinfo.Web.Mvc" />
</application>
<bindings>
    <binding protocol="http" bindingInformation="*:11000:localhost-partnerinfo.tv" />
    <binding protocol="http" bindingInformation="*:80:localhost-partnerinfo.tv" />
    <binding protocol="https" bindingInformation="*:443:localhost-partnerinfo.tv" />
</bindings>
</site>

-------------------------------------------------------------------------------------------------------------------------------

5.) DELETE ALL SETTINGS

netsh http delete sslcert ipport=0.0.0.0:443

netsh http delete urlacl url=http://localhost-partnerinfo.tv:80/
netsh http delete urlacl url=https://localhost-partnerinfo.tv:443/

-------------------------------------------------------------------------------------------------------------------------------

6.) OAUTH APPLICATIONS

FACEBOOK
https://developers.facebook.com/apps

GOOGLE
https://code.google.com/apis/console/#project:753439819819:access

MICROSOFT
https://manage.dev.live.com/Applications/Index

TWITTER
https://dev.twitter.com/apps
