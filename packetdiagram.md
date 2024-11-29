## Packet Structure
## Big endian because the gods said so

MSB                               LSB
_____________________________________
|12345678|12345678|12345678|12345678|
◦--------◦--------◦--------◦--------◦
|version |en| type|  payload length |
◦--------◦--------◦-----------------◦
|         len bytes of data         |
◦-----------------------------------◦