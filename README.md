# CQRSES

Netflix scaling event sourcing
https://www.youtube.com/watch?v=rsSld8NycCU


Authentication as a Microservice
https://www.youtube.com/watch?v=SLc3cTlypwM

Kubernetes in production
https://www.youtube.com/watch?v=-Ci4vd4rh4M&list=PLb0CgirVfdGSUlL7E-qbd8P1jHP0kpqUX



Requirements

User Service
    
    User CreateUser()
    bool UpdateUser(User, data)
    bool DeleteUser(User)

    CreatedUser
    UpdatedUser
    DeletedUser

Auth Service

    JWT Login(User user)
    void Logout(User user)

    LoggedIn
    LoggedOut

Todo Service

    Todo CreateTodo()
    bool UpdateTodo(Todo, data)
    bool DeleteTodo(Todo)

    CreatedTodo
    UpdatedTodo
    DeleteTodo