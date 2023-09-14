UPDATE t1
SET t1.aadhaar_no = t2.aadhaar_no
FROM <your_table_name> t1
JOIN (
    SELECT user_id, MAX(aadhaar_no) AS aadhaar_no
    FROM <your_table_name>
    GROUP BY user_id
) t2
ON t1.user_id = t2.user_id
WHERE t1.aadhaar_no IS NULL;