System42 components
========

This folder contains all of your System42 components. A component is a reusable piece of logic, written in Hyperlambda, 
which you can reuse across multiple apps.

There is one file which most components would declare, which is "startup.hl", expected to do the initialization of your 
component. Usually, if this file exists, it will create the Active Events necessary to consume your component.

This allows you to distribute your System42 using x-copy deployment. By default, System42 contains several pre-built 
components. If you wish to remove these, simply delete the folder containing your component. Preferably _before_ you 
start your server the first time, such that the component's initialization logic does not run.

To see the documentation for your specific components, please refer to the folders inside of this project.

A component can either be a piece of logic, or a reusable widget of some sort, or some other piece of reusable block that
you can utilize in your own projects.

## Ajax Widgets

Many of the components here are reusable Ajax widgets, such as the [DateTimePickers](bootstrap/widgets/datetimepicker) widget, 
the Ajax [TreeView](common-widgets/tree) widget, and so on. This is probably one of the more interesting places to browse around,
if you intend to use Phosphorus Five for creating your own Ajax apps.

## jQuery and Bootstrap CSS

Phosphorus Five included jQuery, which can be, at your convenience, included on your page, in addition to Bootstrap CSS. These
components can be found in this folder.
