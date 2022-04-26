
-- num_tips
UPDATE Business
SET num_tips = 
	(
		SELECT COUNT(*)
		FROM Tip
		WHERE Tip.business_id = Business.business_id
	);

-- num_checkins
UPDATE Business
SET num_checkins = 
	(
		SELECT COUNT(*)
		FROM Check_in
		WHERE Check_in.business_id = Business.business_id
	);

-- total_likes
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
	);

-- fix the nulls
UPDATE Users
SET total_tip_likes = 0
WHERE total_tip_likes IS NULL;

-- tip_count
UPDATE Users
SET total_tip_count = 
	(
		SELECT COUNT(*)
		FROM Tip
		WHERE Users.user_id = Tip.user_id
	);