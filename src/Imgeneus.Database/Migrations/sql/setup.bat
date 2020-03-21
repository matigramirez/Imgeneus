set user_name=root
set password=[your_password]

mysql -u %user_name% --password=%password% < InitSkillData.sql
mysql -u %user_name% --password=%password% < InitItemData.sql