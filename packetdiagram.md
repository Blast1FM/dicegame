## Packet Structure
# Big endian because the netowork order gods said so

MSB                               LSB
___________________________________________
|12345678| 1   23  45678|12345678|12345678|
◦--------◦--------------◦--------◦--------◦
|version |type|met|ident|  payload length |
◦--------◦--------------◦-----------------◦
|         len bytes of data               |
◦-----------------------------------------◦

Octet 1 - Protocol version
Octet 2 - Split between message type, method and resource identifier
Octet 3 - Payload length

# Message type
- 0 - Request
- 1 - Response

# Method type
- 00 - 0 - GET
- 01 - 1 - POST
- 10 - 2 - UPDATE
- 11 - 3 - DELETE

# Resource Identifier

- 00000 - 0 -
- 00001 - 1 -
- 00010 - 2 -
- 00011 - 3 -
- 00100 - 4 -
- 00101 - 5 -
- 00110 - 6 -
- 00111 - 7 -
- 01000 - 8 -

# Payload length
Payload length in bytes. 