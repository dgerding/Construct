﻿
DECLARE @sql AS varchar(max)

SELECT @sql = 'DROP TABLE "' + TABLE_NAME + '"' FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME LIKE 'Items_z%'
SELECT @sql = 'DROP TABLE "' + TABLE_NAME + '"' FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME LIKE 'z_%'

exec(@sql)