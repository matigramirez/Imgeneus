set user_name=root
set password=[your_password]

mysql -u %user_name% --password=%password% < InitSkillData.sql
mysql -u %user_name% --password=%password% < InitItemData.sql
mysql -u %user_name% --password=%password% < InitMobData.sql
mysql -u %user_name% --password=%password% < InitMobItems.sql
mysql -u %user_name% --password=%password% < InitNpcSkillData.sql
mysql -u %user_name% --password=%password% < InitNpcData.sql
mysql -u %user_name% --password=%password% < InitQuestData.sql