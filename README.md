<h2>All Team Members There is the Database File for Implementation of This Project:<h2>
https://drive.google.com/file/d/1IaS6zHgD2yzfyG_MCjaXEZqly8hGazVc/view?usp=sharing

<h2> There is the General Steps or Precautions For Operating The Project: </h2>
<h3> 1) Run This Query In SQL Server After Importing The Database: </h3>
INSERT INTO Tbl_Membership_Plans (plan_id,plan_name, price, duration)
VALUES (1, 'Standard', 35, 6);
INSERT INTO Tbl_Membership_Plans (plan_id,plan_name, price, duration)
VALUES (2, 'Premium', 50, 12);
INSERT INTO Tbl_Membership_Plans (plan_id, plan_name, price, duration)
VALUES (3, 'Free', 0, 1);

<h3> 2) Insert This Membership Status Code in The Tbl_User for Fetching The Details of Membership in User Model To Display it in User Profile: </h3>
public string MembershipStatus { get; set; }
