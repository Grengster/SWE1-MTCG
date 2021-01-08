# MTCG - SWE Zeman 

This is my documentation of my [MTCG][dill].

### Technical Steps Made (designs, failures and selected solutions)

- Start of the Project:

    At first, my goal was to get a functioning Server which is able to handle requests of multiple users by using threads.
This didn't go as expected as there was a problem, I was using Insomnia for sending those requests and Insomnia somehow makes the program create another thread, even if the previous thread is still alive. This caused my server to lose all data of the user class when receiving a new request by an user.
Before I got to the threading part, I ensured that I had my DB set up and fully functional with all the data I would need.
Creating the queries inside the code was the hard part, as there were many conflicts in how I managed the cards.
But the user data was easily done in a few days as I finally found out how to work with the reader of the executer and how to work with multiple results of data from the DB and so on.
I corrected my problem of the threading-data-loss at the end by storing all currently logged users classes inside a dictionary which gets created upon server start. 
    This way, all logged-in users and their decks can easily be handled and accessed. 



    
- Midway during the Project:

    Unlike probably all of my colleagues, I "pushed and pulled" the cards as single json strings inside the DB, where I could easily serialize them for uploading into the DB and then easily de-serialize them for inserting them inside the deck class of the user.
    This was the hardest task and a pitfall as I had to experiment a lot with the JSON strings, classes and how to make them work with each other, in which I finally succeeded.
    I could work with the created dictionary, insert new users, which also created new decks inside them and so on, so it was really tightly combined with everything and I had to be careful to not forget any errors or bugs while working with my more-complex-than-it-needed-to-be-design.

- End of the Project:

    I finally got to the hardest part of the Project, the battle, I experimented a lot without commiting, which was pretty dumb in hindsight, but I got the basics at the end of my project and I am happy how it all turned out at the end.
    I really thought I couldnt get anything done until the deadline but I'm glad that I was wrong.
    


### Time Spent

- ##### Setting up the DB with basic user functions:
    Around 10 hours of work from the 25.12 to the 28.12, with figuring out the commands, syntax, etc.

- ##### Creating the classes, dictionaries and make them successfully work with each other in methods:
    Around 12 hours of work from the 28.12 to the 31.12 while having problems with data-loss (-> lot of time wasted).

- ##### Successfully understanding and using the JSON-Deserializer:
    Around 15 hours of work from the 2.1 to the 4.1 while having lots of problems with the syntax and JSON parser errors, plus creating a list out of a class was new to me, but really helpful.

- ###### Creating the rest functions as viewing decks, inserting cards, buying/setting packages:
    Around 11 hours of work beginning from the 4.1 to the 6.1, it was pretty easy but I had to re-do some query-methods as I had to take care of new bugs, features and again, syntax errors.

- ###### Basic battle:
    Around 10 hours of work from the 25.12 to the 28.12, with figuring out the commands, syntax, etc.


### Plugins

Dillinger is currently extended with the following plugins. Instructions on how to use them in your own application are linked below.

| Plugin | README |
| ------ | ------ |
| Dropbox | [plugins/dropbox/README.md][PlDb] |
| GitHub | [plugins/github/README.md][PlGh] |
| Google Drive | [plugins/googledrive/README.md][PlGd] |
| OneDrive | [plugins/onedrive/README.md][PlOd] |
| Medium | [plugins/medium/README.md][PlMe] |
| Google Analytics | [plugins/googleanalytics/README.md][PlGa] |



[//]: # (These are reference links used in the body of this note and get stripped out when the markdown processor does its job. There is no need to format nicely because it shouldn't be seen. Thanks SO - http://stackoverflow.com/questions/4823468/store-comments-in-markdown-syntax)


   [dill]: <https://github.com/Grengster/SWE1-MTCG>


   [PlDb]: <https://github.com/joemccann/dillinger/tree/master/plugins/dropbox/README.md>
   [PlGh]: <https://github.com/joemccann/dillinger/tree/master/plugins/github/README.md>
   [PlGd]: <https://github.com/joemccann/dillinger/tree/master/plugins/googledrive/README.md>
   [PlOd]: <https://github.com/joemccann/dillinger/tree/master/plugins/onedrive/README.md>
   [PlMe]: <https://github.com/joemccann/dillinger/tree/master/plugins/medium/README.md>
   [PlGa]: <https://github.com/RahulHP/dillinger/blob/master/plugins/googleanalytics/README.md>