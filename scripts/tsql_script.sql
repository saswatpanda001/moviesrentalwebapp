--script1

-- User Management: Insert, Retrieve, Update and Delete Users
DECLARE @Name NVARCHAR(100) = 'John Smith';
DECLARE @Email NVARCHAR(255) = 'john.smith@email.com';
DECLARE @PasswordHash NVARCHAR(255) = 'hashed_password_123';
DECLARE @Phone NVARCHAR(20) = '+1234567890';
DECLARE @Role NVARCHAR(50) = 'User';
DECLARE @UserId INT;

-- Insert new user
INSERT INTO Users (Name, Email, PasswordHash, Phone, Role, CreatedAt)
VALUES (@Name, @Email, @PasswordHash, @Phone, @Role, GETDATE());

SET @UserId = SCOPE_IDENTITY();
PRINT 'New user inserted with ID: ' + CAST(@UserId AS NVARCHAR(10));

-- Retrieve the inserted user
SELECT UserId, Name, Email, Phone, Role, CreatedAt 
FROM Users 
WHERE UserId = @UserId;

-- Update user's phone number
UPDATE Users 
SET Phone = '+1987654321'
WHERE UserId = @UserId;
PRINT 'User phone number updated';

-- Retrieve updated user
SELECT UserId, Name, Email, Phone, Role, CreatedAt 
FROM Users 
WHERE UserId = @UserId;

-- Delete user (commented out for safety)
-- DELETE FROM Users WHERE UserId = @UserId;
-- PRINT 'User deleted';


--script2
-- Movie Management: Insert, Retrieve, Update and Delete Movies
DECLARE @Title NVARCHAR(200) = 'The Matrix Reloaded';
DECLARE @Genre NVARCHAR(100) = 'Sci-Fi';
DECLARE @ReleaseYear INT = 2003;
DECLARE @Description NVARCHAR(1000) = 'Neo discovers the truth about the Matrix and his role in the war against its machines.';
DECLARE @Stock INT = 10;
DECLARE @RentalPrice DECIMAL(5,2) = 3.99;
DECLARE @MovieId INT;

-- Insert new movie
INSERT INTO Movies (Title, Genre, ReleaseYear, Description, Stock, RentalPrice)
VALUES (@Title, @Genre, @ReleaseYear, @Description, @Stock, @RentalPrice);

SET @MovieId = SCOPE_IDENTITY();
PRINT 'New movie inserted with ID: ' + CAST(@MovieId AS NVARCHAR(10));

-- Retrieve the inserted movie
SELECT MovieId, Title, Genre, ReleaseYear, Stock, RentalPrice
FROM Movies 
WHERE MovieId = @MovieId;

-- Update movie's price and stock
UPDATE Movies 
SET RentalPrice = 4.49, Stock = 15
WHERE MovieId = @MovieId;
PRINT 'Movie price and stock updated';

-- Retrieve updated movie
SELECT MovieId, Title, Genre, ReleaseYear, Stock, RentalPrice
FROM Movies 
WHERE MovieId = @MovieId;

-- Delete movie (commented out for safety)
-- DELETE FROM Movies WHERE MovieId = @MovieId;
-- PRINT 'Movie deleted';


--script 3
-- Rental Management: Insert, Retrieve, Update and Delete Rentals
DECLARE @RentalUserId INT = 1;  -- Existing user ID
DECLARE @RentalMovieId INT = 1; -- Existing movie ID
DECLARE @RentalPrice DECIMAL(5,2) = 3.99;
DECLARE @RentalId INT;

-- Check if user and movie exist
IF EXISTS (SELECT 1 FROM Users WHERE UserId = @RentalUserId) 
   AND EXISTS (SELECT 1 FROM Movies WHERE MovieId = @RentalMovieId AND Stock > 0)
BEGIN
    -- Insert new rental
    INSERT INTO Rentals (UserId, MovieId, RentedOn, DueDate, Price)
    VALUES (@RentalUserId, @RentalMovieId, GETDATE(), DATEADD(day, 7, GETDATE()), @RentalPrice);

    SET @RentalId = SCOPE_IDENTITY();
    PRINT 'New rental inserted with ID: ' + CAST(@RentalId AS NVARCHAR(10));

    -- Update movie stock
    UPDATE Movies SET Stock = Stock - 1 WHERE MovieId = @RentalMovieId;

    -- Retrieve the inserted rental
    SELECT r.RentalId, u.Name AS UserName, m.Title AS MovieTitle, 
           r.RentedOn, r.DueDate, r.Price, r.ReturnedOn
    FROM Rentals r
    INNER JOIN Users u ON r.UserId = u.UserId
    INNER JOIN Movies m ON r.MovieId = m.MovieId
    WHERE r.RentalId = @RentalId;

    -- Update rental return date
    UPDATE Rentals 
    SET ReturnedOn = GETDATE()
    WHERE RentalId = @RentalId;
    PRINT 'Rental return date updated';

    -- Update movie stock back
    UPDATE Movies SET Stock = Stock + 1 WHERE MovieId = @RentalMovieId;

    -- Retrieve updated rental
    SELECT r.RentalId, u.Name AS UserName, m.Title AS MovieTitle, 
           r.RentedOn, r.DueDate, r.Price, r.ReturnedOn
    FROM Rentals r
    INNER JOIN Users u ON r.UserId = u.UserId
    INNER JOIN Movies m ON r.MovieId = m.MovieId
    WHERE r.RentalId = @RentalId;

    -- Delete rental (commented out for safety)
    -- DELETE FROM Rentals WHERE RentalId = @RentalId;
    -- PRINT 'Rental deleted';
END
ELSE
BEGIN
    PRINT 'User or movie not found, or movie out of stock';
END



--script 3
-- Rental Management: Insert, Retrieve, Update and Delete Rentals
DECLARE @RentalUserId INT = 1;  -- Existing user ID
DECLARE @RentalMovieId INT = 1; -- Existing movie ID
DECLARE @RentalPrice DECIMAL(5,2) = 3.99;
DECLARE @RentalId INT;

-- Check if user and movie exist
IF EXISTS (SELECT 1 FROM Users WHERE UserId = @RentalUserId) 
   AND EXISTS (SELECT 1 FROM Movies WHERE MovieId = @RentalMovieId AND Stock > 0)
BEGIN
    -- Insert new rental
    INSERT INTO Rentals (UserId, MovieId, RentedOn, DueDate, Price)
    VALUES (@RentalUserId, @RentalMovieId, GETDATE(), DATEADD(day, 7, GETDATE()), @RentalPrice);

    SET @RentalId = SCOPE_IDENTITY();
    PRINT 'New rental inserted with ID: ' + CAST(@RentalId AS NVARCHAR(10));

    -- Update movie stock
    UPDATE Movies SET Stock = Stock - 1 WHERE MovieId = @RentalMovieId;

    -- Retrieve the inserted rental
    SELECT r.RentalId, u.Name AS UserName, m.Title AS MovieTitle, 
           r.RentedOn, r.DueDate, r.Price, r.ReturnedOn
    FROM Rentals r
    INNER JOIN Users u ON r.UserId = u.UserId
    INNER JOIN Movies m ON r.MovieId = m.MovieId
    WHERE r.RentalId = @RentalId;

    -- Update rental return date
    UPDATE Rentals 
    SET ReturnedOn = GETDATE()
    WHERE RentalId = @RentalId;
    PRINT 'Rental return date updated';

    -- Update movie stock back
    UPDATE Movies SET Stock = Stock + 1 WHERE MovieId = @RentalMovieId;

    -- Retrieve updated rental
    SELECT r.RentalId, u.Name AS UserName, m.Title AS MovieTitle, 
           r.RentedOn, r.DueDate, r.Price, r.ReturnedOn
    FROM Rentals r
    INNER JOIN Users u ON r.UserId = u.UserId
    INNER JOIN Movies m ON r.MovieId = m.MovieId
    WHERE r.RentalId = @RentalId;

    -- Delete rental (commented out for safety)
    -- DELETE FROM Rentals WHERE RentalId = @RentalId;
    -- PRINT 'Rental deleted';
END
ELSE
BEGIN
    PRINT 'User or movie not found, or movie out of stock';
END



--script 4
-- Review Management: Insert, Retrieve, Update and Delete Reviews
DECLARE @ReviewUserId INT = 1;  -- Existing user ID
DECLARE @ReviewMovieId INT = 1; -- Existing movie ID
DECLARE @Rating INT = 5;
DECLARE @Comment NVARCHAR(500) = 'Amazing movie! Great visual effects and storyline.';
DECLARE @ReviewId INT;

-- Check if user and movie exist
IF EXISTS (SELECT 1 FROM Users WHERE UserId = @ReviewUserId) 
   AND EXISTS (SELECT 1 FROM Movies WHERE MovieId = @ReviewMovieId)
BEGIN
    -- Insert new review
    INSERT INTO Reviews (UserId, MovieId, Rating, Comment, CreatedAt)
    VALUES (@ReviewUserId, @ReviewMovieId, @Rating, @Comment, GETDATE());

    SET @ReviewId = SCOPE_IDENTITY();
    PRINT 'New review inserted with ID: ' + CAST(@ReviewId AS NVARCHAR(10));

    -- Retrieve the inserted review
    SELECT r.ReviewId, u.Name AS UserName, m.Title AS MovieTitle, 
           r.Rating, r.Comment, r.CreatedAt
    FROM Reviews r
    INNER JOIN Users u ON r.UserId = u.UserId
    INNER JOIN Movies m ON r.MovieId = m.MovieId
    WHERE r.ReviewId = @ReviewId;

    -- Update review rating and comment
    UPDATE Reviews 
    SET Rating = 4, Comment = 'Great movie but the ending could be better.'
    WHERE ReviewId = @ReviewId;
    PRINT 'Review rating and comment updated';

    -- Retrieve updated review
    SELECT r.ReviewId, u.Name AS UserName, m.Title AS MovieTitle, 
           r.Rating, r.Comment, r.CreatedAt
    FROM Reviews r
    INNER JOIN Users u ON r.UserId = u.UserId
    INNER JOIN Movies m ON r.MovieId = m.MovieId
    WHERE r.ReviewId = @ReviewId;

    -- Delete review (commented out for safety)
    -- DELETE FROM Reviews WHERE ReviewId = @ReviewId;
    -- PRINT 'Review deleted';
END
ELSE
BEGIN
    PRINT 'User or movie not found';
END

