# Packet Structure
Big endian because the netowork order gods said so

# V1
```
MSB                                    LSB
___________________________________________
|12345678| 1   23  45678|12345678|12345678|
◦--------◦--------------◦--------◦--------◦
|version |stat|met|ident|  payload length |
◦--------◦--------------◦-----------------◦
|         len bytes of data               |
◦-----------------------------------------◦
```

- Octet 1 - Protocol version  
- Octet 2 - Split between message type, method and resource identifier  
- Octet 3 - Payload length in bytes

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

# ~~V2~~ binned in favour of polling

# Different packet structure for a client request and server message packet

This protocol doesn't strictly follow the request-response pattern.
There are two expected modes of operation:
- Request/Response (default)
- Request/Response with the server being able to send information packets at any point.
The client requests to upgrad to this mode by sending a packet with all the header's 2nd octet bits set.

- Request packet structure
```
MSB                                    LSB
__________________________________________
|12345678|  12  |345678|12345678|12345678|
◦--------◦-------------◦--------◦--------◦
|version |method| ident| payload length  |
◦--------◦-------------◦-----------------◦
|        len bytes of data               |
◦----------------------------------------◦
```
Octet 1 - Protocol version  
Octet 2 - Split between request method and resource identifier  
Octet 3 - Payload length in bytes  

# Method type
- 00 - 0 - GET
- 01 - 1 - POST
- 10 - 2 - UPDATE
- 11 - 3 - DELETE

# Resource Identifier
- Unsigned int between 0 and 62 (inclusive)

# Payload length
- Payload length in bytes.

- Server message packet structure
```
MSB                                  LSB
________________________________________
|12345678| 12345678  |12345678|12345678|
◦--------◦-----------◦--------◦--------◦
|version |status code| payload length  |
◦--------◦-----------◦-----------------◦
|        len bytes of data             |
◦--------------------------------------◦
```
Octet 1 - Protocol version
Octet 2 - Status code
Octet 3 - Payload length in bytes

# Status Codes
- 1x - Info
- 2x - Ok
- 4x - Client error
- 5x - Server error


# Payload length
- Payload length in bytes.