CREATE TABLE Passwords
(
    Password_ID int IDENTITY(1,1) NOT NULL,
    PasswordHash BINARY(64) NOT NULL,
    User_ID int NOT NULL,
    Date_Changed DATETIME
    CONSTRAINT pk_Passwords PRIMARY KEY(Password_ID)
    CONSTRAINT fk_PasswordUsers FOREIGN KEY (User_ID) REFERENCES Users(User_ID) ON DELETE CASCADE
)

CREATE TABLE Users
(
   User_ID int IDENTITY(1,1) NOT NULL,
   First_Name varchar(15) NOT NULL,
   Last_Name varchar(25) NOT NULL,
   Email varchar(100) NOT NULL,
   PasswordHash BINARY(64) NOT NULL,
   CONSTRAINT pk_Users PRIMARY KEY(User_ID)
)

CREATE TABLE Sessions
(
    Session_ID int IDENTITY(1,1) NOT NULL,
    User_ID int NOT NULL,
    Issued DateTime DEFAULT getDate()
    CONSTRAINT pk_Sessions PRIMARY KEY(Session_ID)
    CONSTRAINT fk_Users FOREIGN KEY (User_ID) REFERENCES Users(User_ID) ON DELETE CASCADE
)

CREATE VIEW User_Sessions AS
(
    SELECT Count(Sessions.User_ID) as Number_Of_Logins,
        Users.Email as Email
    FROM Sessions, Users
    WHERE Sessions.User_ID = Users.User_ID
    GROUP BY Sessions.User_ID, Users.Email
)


CREATE OR ALTER TRIGGER PasswordChanged ON Users
AFTER UPDATE
AS
BEGIN
    IF UPDATE(PasswordHash)
    BEGIN
        INSERT INTO Passwords (PasswordHash, User_ID, Date_Changed) 
        SELECT inserted.PasswordHash, inserted.User_ID, getDate()
        FROM Users, inserted
        WHERE Users.User_ID = inserted.User_ID
    END
END

CREATE PROCEDURE Register (@FirstName as varchar(15), 
    @LastName as varchar(35), @email as varchar(100), @password as varchar(25),
    @responseMessage NVARCHAR(250) OUTPUT) AS
BEGIN
    BEGIN TRANSACTION
        BEGIN TRY
        DECLARE @Error NVARCHAR(Max)
    /*Check the user does not exist*/
        DECLARE @UserID INT
        SELECT @UserID = User_ID FROM Users WHERE Users.Email = @email

        IF @UserID is NULL
        BEGIN
        /*All ok, so now you can insert*/
            INSERT INTO Users (First_Name, Last_Name, Email, PasswordHash)
            VALUES(@FirstName, @LastName, @email, HASHBYTES('SHA2_512', @password))
        
            SET @responseMessage = (SELECT SCOPE_IDENTITY())
        END
        ELSE
            SET @responseMessage = '208'

    IF @@TRANCOUNT > 0 COMMIT;
        END TRY
        BEGIN CATCH
            SET @Error = @Error+':An error was encountered : User could not be registered'
            SET @responseMessage = '404'
            IF @@TRANCOUNT > 0
                ROLLBACK TRANSACTION
            RAISERROR(@Error, 1, 0);
        END CATCH 
END

CREATE PROCEDURE ValidateUser(@Email as varchar(100), @Password as varchar(25))
AS
BEGIN 
    BEGIN TRY
        SET NOCOUNT ON

        DECLARE @Error NVARCHAR(Max)
        
        DECLARE @Validated INT
        IF EXISTS (SELECT * FROM Users 
            WHERE Email = @Email AND PasswordHash = HASHBYTES('SHA2_512', @Password) )
            BEGIN
                SET @Validated = 1

                DECLARE @UserID INT
                SELECT @UserID = User_ID FROM Users WHERE Users.Email = @email

                INSERT INTO Sessions(User_ID) VALUES (@UserID)
            END
        ELSE
            BEGIN
                    SET @Validated = 0
            END
 
        RETURN @Validated

    END TRY
    BEGIN CATCH
        RAISERROR(@Error, 1, 0);
    END CATCH 
END


CREATE PROCEDURE DeleteUser(@id as int)
AS
BEGIN
    BEGIN TRANSACTION
        BEGIN TRY
        DECLARE @Error NVARCHAR(Max)
    
        DELETE FROM Users WHERE User_ID = @id;
        IF @@TRANCOUNT > 0 COMMIT;
        END TRY
        BEGIN CATCH
            SET @Error = @Error+':An error was encountered : Deletion could not be carried out'
            IF @@TRANCOUNT > 0
                ROLLBACK TRANSACTION
            RAISERROR(@Error, 1, 0);
        END CATCH 
END


CREATE PROCEDURE UpdateUser(@FirstName as varchar(15), 
    @LastName as varchar(35), @Email as varchar(100), @password as varchar(25), @id as int)
AS
BEGIN
    BEGIN TRANSACTION
        BEGIN TRY
        DECLARE @Error NVARCHAR(Max)

            UPDATE Users 
            SET First_Name = isNull(@FirstName, First_Name),
                Last_Name = isNull(@LastName, Last_Name),
                Email = isNull(@Email, Email),
                PasswordHash = isNull(HASHBYTES('SHA2_512', @password), PasswordHash)
            WHERE User_ID = @id
        
        IF @@TRANCOUNT > 0 COMMIT;
        END TRY
        BEGIN CATCH
            SET @Error = @Error+':An error was encountered : Deletion could not be carried out'
            IF @@TRANCOUNT > 0
                ROLLBACK TRANSACTION
            RAISERROR(@Error, 1, 0);
        END CATCH 
END


/*text for checking a return value

DECLARE @Result INT
EXECUTE @Result = ValidateUser 'shirley.atkinson@plymouth.ac.uk', 'fred'
PRINT @result

*/