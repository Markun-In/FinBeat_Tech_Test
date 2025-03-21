WITH DateIntervals AS (
SELECT
	Id,
	Dt,
	LEAD(Dt) OVER (PARTITION BY Id ORDER BY Dt) AS NextDt
FROM
	Dates
),
GroupedIntervals AS (
SELECT
	Id,
	Dt AS Sd,
	COALESCE(NextDt - INTERVAL '1 day', Dt) AS Ed
FROM
	DateIntervals
WHERE
	NextDt IS NULL OR NextDt - Dt > 1
)
SELECT
    Id,
    Sd,
    Ed
FROM
    GroupedIntervals
ORDER BY
    Id, Sd;