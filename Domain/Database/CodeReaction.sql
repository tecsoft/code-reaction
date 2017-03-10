CREATE TABLE "Comments" ("Id" INTEGER PRIMARY KEY  NOT NULL ,"Revision" INTEGER NOT NULL ,"User" VARCHAR DEFAULT (null) ,"File" VARCHAR DEFAULT (null) ,"LineId" VARCHAR, "Text" VARCHAR DEFAULT (null), "ReplyToId" INTEGER, "IsLike" BOOL);
CREATE TABLE "Commits" ("Id" INTEGER PRIMARY KEY  NOT NULL ,"Revision" INTEGER NOT NULL ,"Author" TEXT NOT NULL ,"Message" TEXT,"Timestamp" DATETIME, "ApprovedBy" TEXT);
CREATE TABLE "Roles" ("Id" INTEGER PRIMARY KEY  AUTOINCREMENT  NOT NULL , "Name" VARCHAR);
CREATE TABLE "UserClaims" ("UserId" INTEGER, "Id" INTEGER, "ClaimType" VARCHAR, "ClaimValue" VARCHAR);
CREATE TABLE "UserLogins" ("UserId" INTEGER, "LoginProvider" VARCHAR, "ProviderKey" VARCHAR);
CREATE TABLE "UserRoles" ("UserId" INTEGER, "RoleId" INTEGER);
CREATE TABLE "Users" ("Id" VARCHAR PRIMARY KEY  NOT NULL  DEFAULT (null) ,"Password" VARCHAR,"Email" VARCHAR,"EmailConfirmed" BOOL,"PasswordHash" VARCHAR,"SecurityStamp" VARCHAR,"PhoneNumber" VARCHAR,"PhoneNumberConfirmed" BOOL,"TwoFactorEnabled" BOOL,"UserName" VARCHAR,"LockoutEndDateUtc" DATETIME,"LockoutEnabled" BOOL,"AccessFailedCount" INTEGER);
