SkypeResearcher
===============

It's easy to look inside skype database and read all the chats and contacts when you have access to the PC. I've made app to make this happen easily. It allows to research internal data structure of the Skype application with SQLite library on C#.
Welcome to the SkypeResearcher wiki!

I found that  Skype uses simple *.db files for data storage! For me it became revelation because many years ago I've read about pretty good security level of Skype safety: it used HTTPS, tricky ip-addresses and a code is strongly obfuscated inside.

However, I successfully connected to Skype database with SQLite library and get access to all tables and data on my local PC. It seems to me a bad style of storage of messages of the user in open form. Your identifiers, guids, the list of contacts and text can be stolen and easily read. Let's come into the folder of UserData and find db-file:
![1](https://optiklab.github.io/img/skype1.jpg)

In fact, in the Skype folder separate folders for each account therefore according to them it is quite possible to learn the list of the accounts ever started on this computer are created.

Simply create connection to the DB file of the biggest size (after all it probably also stores most of all information) and you will find many interesting things. Try yourself with my code on github.
