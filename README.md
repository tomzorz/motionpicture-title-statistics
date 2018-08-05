# 🔠 Motion Picture Title Statistics

## tl;dr; where are the stats?

[Click here.](https://github.com/tomzorz/motionpicture-title-statistics/blob/master/stats.md)

## What is this?

If you're facing the very **theoretical** and **hypothetical** problem that is storing motion pictures in alphabetical order, in many not equally sized storage "areas" you'll find this project helpful.

**As an example:**

You have one storage "areas" of 2 units, 10 units and 1 unit. Where do you "draw the lines" between the letters? 

Answering A-D for 2 units, E-X for 10 units and Y-Z for 1 unit is not really sensible, as there are barely any motion pictures where the title begins with a 'Y' or 'Z'.

This project cleans up all the titles in the source database, and calculates the frequency of each letter at the beginning of the title, allowing you to make an informed decision about your **super hypothetical** problems regarding storage.

## How does it work?

IMDB regularly publishes datasets here: https://www.imdb.com/interfaces/

Running `get-data.ps1` will download and unzip the required source file. Running the .net core project inside the `source` project will generate the `stats.md` file, containing letters, counts and percentages. [👉Click here to see the stats.👈](https://github.com/tomzorz/motionpicture-title-statistics/blob/master/stats.md)

## Everything else

File an issue 🔼above.

_This project is dedicated to /r/datahoarder._