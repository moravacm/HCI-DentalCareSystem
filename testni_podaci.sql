INSERT INTO ZAPOSLENI (JMBG,Uloga,Ime,Prezime,KorisnickoIme,Lozinka,BrojTelefona,Email,Specijalizacija) VALUES
('1111111111111','Administrator','Marija','Moravac','admin','admin123','061123456','admin@ordinacija.com',NULL),
('2222222222222','Stomatolog','Ana','Petrović','ana','lozinka','061234567','ana@ordinacija.com','Opsta stomatologija'),
('3333333333333','Stomatolog','Ivan','Jović','ivan','lozinka','061345678','ivan@ordinacija.com','Ortodontija'),
('4444444444444','Medicinski tehnicar','Maja','Nikolić','maja','lozinka','061456789','maja@ordinacija.com',NULL),
('5555555555555','Medicinski tehnicar','Luka','Popović','luka','lozinka','061567890','luka@ordinacija.com',NULL),
('6666666666666','Stomatolog','Sara','Knezević','sara','lozinka','061678901','sara@ordinacija.com','Endodoncija');

INSERT INTO PACIJENT (JMBG,Ime,Prezime,BrojTelefona,Adresa,Email,ZdravstvenaIstorija) VALUES
('7777777777777','Petar','Petrović','061111111','Ulica 1, Banja Luka','petar@gmail.com','Nema alergija'),
('8888888888888','Milica','Milić','061222222','Ulica 2, Banja Luka','milica@gmail.com','Alergija na penicilin'),
('9999999999999','Jovan','Jovanović','061333333','Ulica 3, Prijedor','jovan@gmail.com','Nema istorije'),
('1010101010101','Ivana','Ivanović','061444444','Ulica 4, Doboj','ivana@gmail.com','Visok pritisak'),
('1212121212121','Marko','Marković','061555555','Ulica 5, Banja Luka','marko@gmail.com','Dijabetes'),
('1313131313131','Ana','Anić','061666666','Ulica 6, Banja Luka','ana@gmail.com','Nema alergija');

INSERT INTO KATEGORIJA (Naziv) VALUES
('Anestetici'),
('Punila i kompoziti'),
('Endodontski materijali'),
('Instrumenti za jednokratnu upotrebu'),
('Materijali za otiske');

INSERT INTO MATERIJAL 
(idKategorije, Naziv, JedinicaMjere, CijenaPoJedinici, MinimalnaZaliha, TrenutnaZaliha) VALUES
(1, 'Lidokain 2%', 'ampula', 2.50, 20, 35),
(1, 'Articain 4%', 'ampula', 3.20, 15, 25),
(2, 'Kompozit A2', 'g', 1.80, 50, 80),
(2, 'Kompozit A3', 'g', 1.80, 50, 45),
(3, 'Gutaperka konusi', 'pakovanje', 5.50, 10, 18),
(3, 'Srebro nitrata', 'ml', 4.00, 5, 7),
(4, 'Igle 27G', 'kom', 0.10, 100, 250),
(4, 'Plastične čašice', 'kom', 0.05, 200, 500),
(5, 'Alginat', 'g', 0.15, 500, 900),
(5, 'Silikonska masa', 'g', 0.25, 300, 450);

INSERT INTO TRETMAN (Naziv,Cijena) VALUES
('Kontrola rutinska',30.00),
('Kontrola karijesa',40.00),
('Kontrola ortodontska',50.00),
('Kontrola endodontska',60.00),
('Plomba kompozitna',40.00),
('Plomba amalgam',60.00),
('Skidanje kamenca',50.00),
('Parodontološki tretman',120.00),
('Ekstrakcija zuba',150.00),
('Stomatološka intervencija',200.00),
('Izbjeljivanje zuba',200.00),
('Fiksna proteza',1200.00),
('Popravka zuba', 50.00),
('Hirursko vađenje zuba', 100.00);

INSERT INTO TERMIN (DatumIVrijeme,JMBGPacijenta,JMBGStomatologa) VALUES
('2025-09-01 09:00:00','7777777777777','2222222222222'),
('2025-09-01 10:00:00','8888888888888','2222222222222'),
('2025-09-01 11:00:00','9999999999999','3333333333333'),
('2025-09-01 12:00:00','1010101010101','6666666666666'),
('2025-09-02 09:00:00','1212121212121','3333333333333'),
('2025-09-02 10:00:00','1313131313131','6666666666666');

INSERT INTO PREGLED (Dijagnoza,idTermina,JMBGMedicinskogTehnicara) VALUES
('Karijes','1','4444444444444'),
('Parodontoza','2','4444444444444'),
('Ortodoncija','3','5555555555555'),
('Endodoncija','4','5555555555555'),
('Kontrola rutinska','5','4444444444444'),
('Kontrola specijalna','6','5555555555555');

INSERT INTO TRETMAN_STAVKA(idTretmana,idPregleda,Cijena) VALUES
(1, 1, 30.00),
(2, 1, 40.00),
(1, 2, 30.00),
(5, 3, 40.00),
(7, 4, 50.00),
(1, 5, 30.00),
(13, 6, 50.00);
