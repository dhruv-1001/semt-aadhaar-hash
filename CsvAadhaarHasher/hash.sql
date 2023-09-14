-- Not used anywhere

ALTER TABLE UserAadhaar
ADD hashed_aadhaar VARCHAR(64);

DECLARE @BatchSize INT = 100000;
DECLARE @TotalRows INT = (SELECT COUNT(*) FROM UserAadhaar);
DECLARE @ProcessedRows INT = 0;

WHILE @ProcessedRows < @TotalRows
BEGIN
    UPDATE TOP (@BatchSize) UserAadhaar
    SET hashed_aadhaar = HASHBYTES('SHA2_256', aadhaar_no)
    WHERE hashed_aadhaar IS NULL;
    SET @ProcessedRows = @ProcessedRows + @@ROWCOUNT;
    COMMIT;
END


BULK INSERT StagingUserAadhaar
FROM 'C:\Path\To\Your\HashedAadhaar.csv'
WITH (
    FIRSTROW = 2,  -- Skip the header row if present
    FIELDTERMINATOR = ',',  -- Comma-separated values
    ROWTERMINATOR = '\n',   -- Newline row terminator
    BATCHSIZE = 10000       -- Adjust the batch size as needed
);

update your_table
set some_column = case when id = 1 then 'value of 1'
                       when id = 5 then 'value of 5'
                       when id = 7 then 'value of 7'
                       when id = 9 then 'value of 9'
                  end
where id in (1,5,7,9)