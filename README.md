# DatingApp-net8
DatingApp is an application implemented using the dotnet core 8 and angular. for development purpose we have been using the Sqlite Database.
Entity Frame work:

An object relational Mapper(ORM)
Translates our code into SQL commands that update our tables in the database.
When we add entity framewwork, we need to create an important class that derives from the DBContext class, which comes with EF .

This class acts as a bridge between our domain or entity classes and the database

DBcontext class is the primary class we use for interacting with our database.

Example: 
Entity framework, allows us to use linqqueries which are similar as follows.

DBContext.Users.Add(new User{ Id=4, UserName= Lakshmi}) --- > SQliteProvider --> INSERT INTO Users(Id, Name) Values (4, Lakshmi)


Featues:
 Querying
 Change Tracking
 Saving(Insert, update,delete) -- Save changes method
 Concurrency
 Transactions
 Caching 
 Built- in conventions  (EF schema for ID )
 Configurations
 Migrations

 when we want to use a class from different name space , we use using statements
 A DBContext instance represens a session with the database and  can be used to query and save instances of our entities.
 DB context  is a combination of the unit of work and repository pattern

 A DbSet can be used to query and save instances of TEntity. 
 LINQ queries against a DbSet will be translated into queries against the database.


[ApiController]
Indicates that a type and all derived types are used to serve HTTP API responses.

Controllers decorated with this attribute are configured with features and behavior targeted at improving the developer experience for building APIs.

When decorated on an assembly, all controllers in the assembly will be treated as controllers with API behavior.

Authentication:
1. storing in the clear text --use ssl security to encrypt the data
2.  Hashing the password

var user=  _repo.Users.Find(username);
 var passwordHash= CalculatePasswordHash(password);
 if(passwordHash!=user.PasswordHash){
    return UnAuthorized
 }

 Problems with the Using the PasswordHash:
1. if two users use the same password, then their PasswordHashes will be same. 
this gives an hint on decode the password to the attackers. If database is compromised.
 2. Using the just hashing a password -- there are dictionaries of every possible commonly used or 
 combination of different passwords available, and their hashes have been already beeen computed
 in various different algorithms.
 they can check the hashes and decode the password.
3. Hashing and salting the password.
// PasswordSalt is a randomised string passed to the CalculatePasswordHash to randomise the password hash.
even though the users have same passwords , the hashes will be different.
var user =_repo.Users.Find(username);
var passwordHash=CalculatePasswordHash(password,user.PasswordSalt);
if(passwordHash!=User.PasswordHash){
    return UnAuthorized;
}

JSON Web Tokens -- JWT 
how to authenticate an Api as doesnt maintain the session state.
 tokens are good to use with in the Api
 Industry standard tokens 
 self contained 
 It contains credentials
 claims
 other information
Basically long string seperated by the period(.)
 JWT Structure
 Header of the token: Algorithm and type
 {"alg" : "algorithm code", "typ":"JWT}
Payload (Data)
claims-- claiming to be something
"nameid:,"role","nbf", "exp", "iat"
nbf- not before 
exp- expiry time
iat- issued at the time

Verify Signature
Signature is encrypted. -- by the server itself.

send username +Password 

validate credentials and return a JWT token (that the client will store locally on their machine)
--- typically we often use browser storage to hold on to the token so that 
---we can send the JSON web token with every single request

sends JWT with further requests(Authentication header to the request)
-- then the server will take a look at the token and verify that the token is valid.
-- the server, that signed the token will have access to the private key thats stored on the server
-- is able to verify that the token is valid without needing to make a call to a database.

server verifies JWT and sends back the response.

Benefits :
No session to manage - JWTs are self contained tokens
Portable- A single token can be used with multiple backends
No cookies required- mobile friendly
Performance- Once a token is issued, there is no need to make a database request to verify a users authenticationn.

DependencyInjection --
Lifetimes - singleton,Transient,Scoped
AddCors,AddDbContext all are built-in services 
when custom services is being injected, we need to tell the dotnet how long we want a service to live for.

AddSingleton-- singleton services are created the first time they are requested and then every subsequent request
                for that service will use the exact same instance.
                --this type of lifetime is good for when we want to cache data or 
                  maintain a state that should be shared across the whole application.

AddTransient --- Transient services are created each time they are requested from the service container.
                lightweight and stateless services

AddScoped ---these services are created once per the client request(Http request).

Change the allowedhosts from appsettings .json to https:localhost/5000,

Need to install , 2 nuget packages
Microsoft.AspNetCore.Authentication.JwtBearer -- to authenticate the user
System.IdentityModel.Tokens.Jwt -- To create the token and encrypt the key.

use the descriptor --[Authorize] [AllowAnonymous]

after generating the jwt token , 
we need to authenticate the APIs which and all needs to be authorized 
so, we need to add in the request pipeline (Middleware) -- Add the Authentication and once after the build is done.
UseAuthentication(), and UseAuthorization()


OnInit:

A lifecycle hook that is called after Angular has initialized all data-bound properties of a directive. 
Define an ngOnInit() method to handle any additional initialization tasks.

http=inject(HttpClient) --- Dependency injection

observable needs to be subscribed to perform the action 
http.get () -- GET request that interprets the body as JSON and returns the response body as an object parsed from JSON
should unsubscribe once subscribed to observable (otherwise it is a memory consuming and resource killing process.), 
http request will definetly unsubscribe once the request is completed.

structural Directive -- ngFor

@for (user of users ; track index) -- is a direct flow of angular.

two types of forms - Angular forms and reactive forms.

observables and signals:
new standard for managing async data included in ES7. - Introduced in angular v2.
Observables are lazy collectionsof mltiple values over time.
You can think of observables like a newsletter:
-- Only subscribers of the newsletter recieve the newsletter.
-- If no-one subscribes to the newsletter , it probably will not be printed.
effectively, if we want to listen to an observable and get those values over time, we have to subscribe to them.
if we want to stop listening , then have to unsubscribe.

Promises vs Observables
Provides a single future value.
It is not lazy and always return something even atleast error.
can not cancel.

Emits multiple values over time and best for the data streaming.
Lazy
Able to cancel 
Can use the map, filter, reduce and other operations using library Rxjs.

RxJs.  -- we can use map function (pipe-- have to use to get the operators from the rxjs )
what to do next
what to do if there is  an error
what to do when completed.
Http requests are always complete. 
can use ToPromise () function to convert to promise.
Async Pipe:
<li *ngFor='let member of service.getMembers() | async'>{{member.username}}</li>
this will automatically , subscribe and unsubscribe from the observable.

Signal:
to notify the components of other components using them.
A signal is a wrapper around a value that notifies interested consumers when that value changes.
Signals can contain any value, from primitives to complex data structures.
signals are getter functions , calling them reads their value.

Now any other component thats is using these signals, that component would be notified of that update.
These works briliant with the synchronous code .
Eg: whether user is logged in or not.

Advantages:
Simplicity and readability(great for state management)
Performance. (leads to less unneccessary updates, automatic memory cleaning)
Predictability(predictable state changes as update synchronously)
Integration( integrated with the angular)





commands:
nvm -v
nvm install lts
nvm list
nvm use newest

node -v
dotnet dev-certs https --trust
dotnet run
dotnet tool install --global dotnet-ef --version 8.0.11
dotnet tool list -g
dotnet ef
dotnet ef migrations -h
dotnet ef migrations add InitialCreate -o Data/Migrations
dotnet ef database -h
dotnet ef database update
dotnet watch
dotnet new list
dotnet new gitignore

git add .
git commit -m " "
git remote add origin "repository link.git"
git pull
git push
git push --set-upstream origin main

git branch -m main
git status
git log
git pull --tags origin main --allow-unrelated-histories

ng serve

creating the self signed certificates 
ng --version
choco install mkcert
mkcert -install
mkdir ssl
cd ssl
// now install the certificate
mkcert localhost

After installing the certificate update the angular .json file for serve with options 
and add the ssl path, sslCert, sslKey paths to the file.




