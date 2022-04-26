
-- a. when user provides tip (aka INSERT INTO Tip), Business.num_tips and Users.total_tip_count updated

CREATE OR REPLACE FUNCTION UpdateNumTips() RETURNS trigger AS 
'
BEGIN

    UPDATE Business
    SET num_tips =
        (
            SELECT COUNT(*)
            FROM Tip
            WHERE Tip.business_id = Business.business_id
        )
    WHERE NEW.business_id = business_id;

    UPDATE Users
    SET total_tip_count =
        (
            SELECT COUNT(*)
            FROM Tip
            WHERE Tip.user_id = Users.user_id
        )
    WHERE NEW.user_id = user_id;
	
    RETURN NEW;

END
' 
LANGUAGE plpgsql;
    

CREATE OR REPLACE TRIGGER NumTips
AFTER INSERT ON Tip
FOR EACH ROW
EXECUTE PROCEDURE UpdateNumTips();

-- b. when customer checks in to a business, num_checkins should be updated

CREATE OR REPLACE FUNCTION UpdateNumCheckins() RETURNS trigger AS 
'
BEGIN

    UPDATE Business
    SET num_checkins =
        (
            SELECT COUNT(*)
            FROM Check_in
            WHERE Check_in.business_id = Business.business_id
        )
    WHERE NEW.business_id = business_id;

    RETURN NEW;

END
' 
LANGUAGE plpgsql;

CREATE OR REPLACE TRIGGER NumCheckins
AFTER INSERT ON Check_in
FOR EACH ROW
EXECUTE PROCEDURE UpdateNumCheckins();

-- c. when a random user likes tip, the user who wrote it should have their total_tip_likes updated

CREATE OR REPLACE FUNCTION UpdateTipLikes() RETURNS trigger AS 
'
BEGIN

    UPDATE Users AS U1
    SET total_tip_likes = 
        (
            SELECT SUM(likes) AS sum_likes
            FROM
            (
                SELECT Tip.user_id, Tip.business_id, likes
                FROM Tip, Users
                WHERE Tip.user_id = Users.user_id
            ) AS User_Tips
            WHERE U1.user_id = User_Tips.user_id
            GROUP BY user_id
        )
    WHERE NEW.user_id = user_id;

    RETURN NEW;

END
' 
LANGUAGE plpgsql;

CREATE OR REPLACE TRIGGER NumCheckins
AFTER UPDATE ON Tip
FOR EACH ROW
EXECUTE PROCEDURE UpdateTipLikes();