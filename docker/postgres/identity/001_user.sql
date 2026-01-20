create table Users (
    Id number primary key,
    Email text not null unique,
    PasswordHash text not null,
    CreatedAt timestamptz not null
)

create index IX_Users_Email on Users(Email)