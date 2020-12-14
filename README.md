# General

    Basic Cart webApi with two entities : product and cartItem.

    The main entity (cartItem) is extendable through its base.

    User can achive a product list, select a specific product , add a product(and it's quantity) to the cart ,
    update the quantity, remove product from the cart, and clear the cart.

    I have added a swagger support https://localhost:5001/swagger/index.html

# Concepts

    I'm using Entity framework inMemoryDb to store data.

    I'm taking adventage of the async concept both in the webApi and over the EF async operations where needed.

# Assumptions

    As this is a webApi (each request is carring its own execution context) and because there is no need to share static data (at this point ) ,im not using currently any thread safe approach.

    Another thing which i'm not implementing right now is transactions over db operations.
    Right now at this implementation there is no complex/structured queries and i'm mainly using basic CRUD's actions so no partial change can occur.

# TBD
    Implement a message broker (Kafka,RabbitMq...) and subscribe to a channel for sending the cart to the payment service.
    (I've added a demi "checkout" operation that demonstrate the concept, when a cart is being published it is also being updated to non active for a history tracking)

    Publish to Docker hub (or any other containder) .
    

