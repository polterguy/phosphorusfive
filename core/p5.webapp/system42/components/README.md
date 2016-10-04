Components
========

This folder contains all of your System42 "components". A component is a reusable piece of logic, written in Hyperlambda, 
which you can reuse across multiple apps. Think "COM, the Hyperlambda version".

There is one file which most components would declare, which is "startup.hl", expected to do the initialization of your 
component. Usually, if this file exists, it will create the Active Events necessary to consume your component.

This allows you to distribute your System42 using x-copy deployment. By default, System42 contains several pre-built 
components. If you wish to remove these, simply delete the folder containing your component. Preferably _before_ you 
start your server the first time, such that the component's initialization logic does not run.

