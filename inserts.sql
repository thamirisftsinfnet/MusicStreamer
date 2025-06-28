-- Inserir Bandas (sem Id)
INSERT INTO Bands (Name, Description, ImageUrl, CreatedAt) VALUES
('The Rolling Codes', 'Banda de rock clássica voltada a devs.', 'https://example.com/rolling.jpg', GETUTCDATE()),
('NullPointer Beats', 'Banda de synthwave binária.', 'https://example.com/nullpointer.jpg', GETUTCDATE());

-- Inserir Álbuns (usando IDs das bandas já inseridas)
INSERT INTO Albums (Title, ReleaseDate, CoverImageUrl, BandId) VALUES
('Code Revolution', '2020-05-01', 'https://example.com/revolution.jpg', 1),
('Legacy Bug Fixes', '2021-08-10', 'https://example.com/legacy.jpg', 1),
('Bytewave', '2022-09-12', 'https://example.com/bytewave.jpg', 2),
('Recursive Beats', '2023-01-20', 'https://example.com/recursive.jpg', 2);

-- Inserir Músicas (sem Id)
INSERT INTO Musics (Title, Duration, FileUrl, TrackNumber, AlbumId) VALUES
('Refactor My Heart', '00:03:45', 'https://example.com/music/refactor.mp3', 1, 1),
('Commit to You', '00:04:12', 'https://example.com/music/commit.mp3', 2, 1),
('Null Exception', '00:03:22', 'https://example.com/music/null.mp3', 1, 2),
('While(true) Love', '00:04:10', 'https://example.com/music/whiletrue.mp3', 2, 2),
('Stack Overflow Dreams', '00:05:02', 'https://example.com/music/overflow.mp3', 1, 3),
('Garbage Collector', '00:02:55', 'https://example.com/music/gc.mp3', 2, 3),
('Echoes of RAM', '00:04:35', 'https://example.com/music/echoes.mp3', 1, 4),
('Final Return', '00:03:50', 'https://example.com/music/return.mp3', 2, 4);
