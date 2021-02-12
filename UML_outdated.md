### Models

#### SHARED

**Interfaces for communication**

interface ComModel

- +type : byte;
- +toCharArray()

interface SendComModel implements ComModel

- +memberModel : MemberModel; 



class MemberModel

 + -UserId : UUID nullable
 + -name : char [ ]
 + -ns : NetworkStream
 + +MemberModel(string name, NetworkStream ns)
 + +sendMessage(SendComModel message)



interface MessageModel implements ComModel

- +timestamp: Timestamp

- +text : char[]

interface SendMessageModel implements MessageModel, SendComModel 



#### CLIENT

**Chat Models**

ClientRecieveMessageModel implements MessageModel 

ClientSendMessageModel implements SendMessageModel 

**Handshake and social Models**

ClientSendHandshakeModel implements SendComModel 

-  -name

ClientRecieveMemberListModel implements ComModel

- -members : char[ ] [ ] 

#### SERVER



**Chat Models**

ServerRecieveMessageModel implements MessageModel 

ServerBroadcastMessageModel implements SendMessageModel

**Handshake and social Models**

ServerRecieveHandshakeModel implements ComModel

- name

ServerSendMemberListModel implements SendComModel 

- members : char[ ] [ ] 

### message protocol 

Forbidden chars to type in textbox: 

begin chars ^^^

data separator chars ~~~

end chars  $$$

Every message starts with ^^^, data is seperated by one or multiple ~~~ and ended with $$$. This guarantees the message will work on various buffersizes.

A sample message of the type ServerSendMemberListModel might look like this:

```^^^13~~~member1~~~member2$$$``` 