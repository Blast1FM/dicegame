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

# Status Code
- 0 - Ok
- 1 - Error

# Method type
- 00 - 0 - GET
- 01 - 1 - POST
- 10 - 2 - UPDATE
- 11 - 3 - DELETE

# Resource Identifier
- Unsigned int between 0 and 31 (inclusive)

# Payload length
- Payload length in bytes.
