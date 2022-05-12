SELECT p.PeakName, r.RiverName, 
	LOWER(CONCAT(LEFT(p.PeakName, LEN(p.PeakName) - 1), r.RiverName)) as Mix
	FROM Peaks as p, Rivers as r
	WHERE RIGHT(p.PeakName, 1) = LEFT(r.RiverName, 1)
	ORDER BY Mix