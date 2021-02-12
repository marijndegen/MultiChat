### message protocol 

Forbidden chars to type in textbox: 

begin chars ^^^

data separator chars ~~~

end chars  $$$

Every message starts with ^^^, data is seperated by one or multiple ~~~ and ended with $$$. This guarantees the message will work on various buffersizes.

A sample message of the type ServerSendMemberListModel might look like this:

```^^^13~~~member1~~~member2$$$``` 