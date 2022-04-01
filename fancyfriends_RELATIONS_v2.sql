CREATE TABLE Users (
	user_id CHAR(22),
	first_name VARCHAR,
	last_name VARCHAR,
	total_tip_count INT DEFAULT 0,
	total_tip_likes INT DEFAULT 0,
	average_stars FLOAT,
	cool INT,
	funny INT,
	useful INT,
	date_joined DATE,
	number_of_fans INT DEFAULT 0,
	longitude FLOAT,
	latitude FLOAT,
	count_of_votes INT,
	PRIMARY KEY (user_id)
);

CREATE TABLE Business (
	business_id CHAR(22),
	name VARCHAR,
	num_checkins INT DEFAULT 0,
	stars INT,
	num_tips INT DEFAULT 0,
	state CHAR(2),
	city VARCHAR,
	zip_code INT,
	is_open BOOL,
	latitude FLOAT,
	longitude FLOAT,
	address VARCHAR,
	PRIMARY KEY (business_id)
);

CREATE TABLE Tip(
	user_id CHAR(22) NOT NULL,
	business_id CHAR(22) NOT NULL,
	tip_time TIMESTAMP,
	tip_text VARCHAR,
	likes INT,
	PRIMARY KEY (user_id, business_id, tip_time),
	FOREIGN KEY (user_id) REFERENCES Users(user_id),
	FOREIGN KEY (business_id) REFERENCES Business(business_id)
);


CREATE TABLE Attribute (
	name VARCHAR,
	value VARCHAR,
	business_id CHAR(22),
	PRIMARY KEY (name, business_id),
	FOREIGN KEY (business_id) REFERENCES Business(business_id)
);

CREATE TABLE Category (
	name VARCHAR,
	business_id CHAR(22),
	PRIMARY KEY (name, business_id),
	FOREIGN KEY (business_id) REFERENCES Business(business_id)
);

CREATE TABLE Hours (
	business_id CHAR(22),
	day VARCHAR,
	open_time TIME,
	closing_time TIME,
	PRIMARY KEY (business_id, day),
	FOREIGN KEY (business_id) REFERENCES Business(business_id)
);

CREATE TABLE Check_in (
	business_id CHAR(22),
	check_in_time TIMESTAMP,
	PRIMARY KEY (business_id, check_in_time, day, month, year),
	FOREIGN KEY (business_id) REFERENCES Business(business_id)
);


CREATE TABLE Friends (
	user_id CHAR(22),
	user_id2 CHAR(22),
	PRIMARY KEY (user_id, user_id2),
	FOREIGN KEY (user_id) REFERENCES Users(user_id),
	FOREIGN KEY (user_id2) REFERENCES Users(user_id)
);