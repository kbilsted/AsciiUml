# AsciiUml

A drawing program for making diagrams in ascii.

Ascii diagrams have many advantages over traditional diagrams

  * Easy to modify
  * Easy to paste into wiki's, chat's etc
  * Easy to search for elements within a diagram (eg. CTRL-F)
  * Diagrams are less formal due to the crudeness of ASCII, making it easier to spark discussions

![asciiuml in action](/documentation/asciiuml.gif)

To save a picture simply select the text and press copy.



## Features
 
  * Boxes
  * Labels
  * Arrows (shortest path, turn minimization)
  * Free style arrows
  * Move objects
  * Resize objects


## Help wanted
This is a **fun** project. While simple on the outside, there are some advanced things going on under the hood. It would be cool to form a community around the tool and help make it more mature. I certainly have plenty of ideas for ways of improving it :-)

If you want to help out just raise an issue here on github with your ideas.
  

## Architecture

The code base is a bit of a mess. Styles are mixed. Initially, heavily use of functional code and the "functional core, imperative shell" pattern. It prooved to be more difficult as a GUI library was introduced. So now the code is becomming more OOP. 

Given a state and a key press, a new state is returned, which in turn is printed to the screen. The new state then serves as the state and yet another key press is awaited..

Lets try to draw this using AsciiUml itself

```
*******************------------>****************
* KeyboardHandler *             * Draw service *
*******************             ****************
       ^          |
       |          |
       |          |
       |          |
 state |          | new state
       |          |
       |          |
       |          |
       |          v
       ************
       * EvalLoop *
       ************
```



## NOTICE

The state of the project very alpha. It will crash upon use! :-) 
