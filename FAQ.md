# FAQ

### How do I get support?

[By submitting an issue here](https://github.com/polterguy/phosphorusfive/issues)

### How do I report a bug?

[By submitting an issue here](https://github.com/polterguy/phosphorusfive/issues)

### How do I request a feature?

[By submitting an issue here](https://github.com/polterguy/phosphorusfive/issues)

### How do I submit a patch?

You don't. For legal reasons I cannot accept patches to Phosphorus Five. You can
fork it, submit a bug report, tap into its plugin architecture, and probably
change any aspect of it without rendering your code incompatible - But I cannot
accept patches to it. If you can accurately describe your problem, preferably
with a simple to use reproducible, I would love to help you out. I also deeply
appreciate suggestions.

### Where do I find the changes document?

[Here](CHANGES.md). Notice, this document is appended to as I implement the changes,
and there might be items in it, which has not yet been released. However, this allows
you to see what is coming up in the next release, and also which changes you can expect
to see, if you clone the repository instead of downloading the latest zip file.

**Warning** - Some of these changes have not yet been stabilized, and the raw Git repository
should not be considered _"stable"_. Have this in mind if you choose to clone the
repository, instead of downloading the latest release.

### How do I create a source code release myself?

By installing _"Cake build"_, which can be done by executing one of the following
commands in a terminal window, from the root of your (Source Code) Phosphorus Five
folder - Depending upon which native operating system you are on.

* __Windows__ - `Invoke-WebRequest https://cakebuild.net/download/bootstrapper/windows -OutFile build.ps1`

* __Linux__ - `curl -Lsfo build.sh https://cakebuild.net/download/bootstrapper/linux`

* __OS X__ - `curl -Lsfo build.sh https://cakebuild.net/download/bootstrapper/osx`

Then make sure your _"build.sh"_ script becomes an executable (Linux + OS X only)
by executing the following terminal command. This makes your build script become
an executable. This only applies for Linux and Windows.

```
chmod +x build.sh
```

Then execute the following command from a terminal window (yet again from the
root folder of your Phosphorus Five Source Code folder).

```
./build.sh
```

Or if on Windows ...

```
./build.ps1
```

The above will create a _"Source-Complete-With-Submodules.zip"_ file for you,
being a source code release of the entire system, including its documentation, and
the build script itself. This process will create a Source Code _only_ release
of your Phosphorus Five system.

**Notice**, this process will take some time. Please let it finish.

### How do I deploy a release build into my production server?

The easiest way is to purchase a proprietary license, and using _"Hyperbuild"_,
which allows you to _"clone"_ (parts of) your existing installation.

**Notice**, make sure you use the _"Release"_ build configuration from Visual
Studio/Xamarin/MonoDevelop when you start your debugger, since otherwise
you'll end up deploying a debug version of your system, which is probably not
optimal!

### Who created Phosphorus Five?

Every single line of code was created by me (Thomas Hansen), except where I use
libraries, and/or other external free software resources, which I explicitly
mark as such.

### Who owns the copyright for Phosphorus Five?

It is in its entirety owned by Thomas Hansen, in person, me that is, which is a
conscious choice, since it gives me a higher degree of security and freedom.

### Can I invest in Phosphorus Five?

__NO!__ Go away, I don't want to talk to you, and I don't intend to explain why either.
Go buy BitCoin or something ...

### Can I ask you to hold a course for me and my developers?

Sure, I do this all the time, depending upon my schedule. Shoot me an email at
thomas@gaiasoul.com, and I'll check my schedule, and try to squeeze you in.
It'll cost you though, and I expect you to pay all my expenses, in addition
to my usual fee. For reasons I don't intend to explain here though, I don't travel
to the USA.

### Are you the guy playing Saxophone, Harmonica, Guitar, Singing on your videos?

Yup! My main hobbies, besides traveling, includes creation of music, and I almost plays equally
many instruments, as I know programming languages.

### How on Earth did you do this? (Create Phosphorus Five)

I spent 13 years researching the same idea, and I followed Winston Churchill's advice,
which goes like this ...

> Never, ever, ever, ever, ever, ever, ever, ever, ever, ever, ever, ever, ever, ever, ever, ever, ever, ever, ever give up!

Then mix in a little bit of brilliance, and a little bit of madness - And __VOILA!__
Phosphorus Five comes out in the other end ...

### Who controls the development of Phosphorus Five?

I am the Gaia Lactical Overlord, and Ajax Uber Uhuru Guru, Il Duce, Der Fuhrer,
and Kim Jong Il the 3rd when it comes to the final decisions in regards to the future development
of Phosphorus Five. 100% dictator, the resurrected Kublai Khan, and sole emperor over Phosphorus Five.

I do however take suggestions and feedback from others, and I do consider myself quite polite, and I am
famous for changing my mind, once my existing mind no longer compiles.

### What's your email address?

thomas@gaiasoul.com

### Can I sponsor the project?

Sure, if you represent a company, and or some individual with a deep wallet, and
you'd like to be represented as an _"official sponsor"_ to the project - Feel free
to shoot me an email at thomas@gaiasoul.com. I am in the process of setting up
a general sponsor thing, allowing for sponsors to be mentioned in the documentation
for the system, with a small pitch about their products and services, and a link
to their website. Which might (or might not) get you more customers to your products
and services.

### Can I resell Phosphorus Five?

[Yup](https://www.youtube.com/watch?v=kFUSenWxrOM)!

### Can I buy you a cup of coffe, a dinner, or a new Harley Davidson?

[Sure](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=BXLLPEF2AG7VC), thank you :)
