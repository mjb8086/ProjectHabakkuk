#!/usr/bin/env perl
use v5.32;
use warnings;

my @FORENAMES_MALE = qw(
Andrew Aaron Arnold Ainsley Antony Alexander 
Alf Archer Alfred
Bryan Bryson Bernard Bruce Ben Barry Bill
Christopher Carl Cyril Ciaran Colin
David Daniel Damon
Edward Edgar Ethel Ebenezer Eric
Felix Fontaine
George Glenn Godwin Graham Gerald
Harold Hubert Hugh Horace Humphery
Ian
Joseph James Jonathan
Kenneth
Lawrence Leslie Llaim Louis Linus
Michael Mark Matthew Maximillian Morris Mortimer
Nicholas Neville
Paul Peter Philip Patrick
Quentin
Robert Ryan Roger Rowan Robin
Simon Stanley Samuel Stephen
Terrance Tom
Vincent
William Winston
);

my @FORENAMES_FEMALE = qw(
Amy Annette Anita Alice Allison
Bernadette Beatrice
Carolyn Christine Ciara Claire Catherine Carrie Charlene Chloe
Doreen Danielle Deborah
Ellenor Emma Evangeline Elizabeth
Felicity
Georgina Gail Gillian
Hannah Heather
Jennifer Joanne
Kathlin
Linda Lilly Lorraine Laura
May Maureen Monica Madeline Marian Maxine
Ophelia
Penny Pearl
Rachel Rebecca Ruth
Stephanie Susan
Vivian
Wynona
Yvonne
);

my @SURNAMES = qw(
Anderson Armstrong Alburn Alder Ashen Archibald Archer Appleby
Bates Brown Bolton Boyce Boyd Blair
Connolly Columbus Caroll Chamberlain Claypool Coxon Currie Carruthers
Crossett Crooks Cutter Cage Cooke Chapman Cartman
Dobbin Dell Delvin Davis Doyle Defoe
Erskin
Forsythe
Griffin Goodman Green
Hyde Hughes Hamilton
Kane
Laverty Lumberg Livingstone Livesay Locke
Mitchell Moorehead Moore Montgomery McMullan McCallion McCullogh Morrison McGlone
Norris Newton
Oldman Overend O'Leary
Parke Pollard Paulson
Ramsey Rochefort Robinson Ramseybaggs
Smith Stuart Steele Smyth Stone Stallman
Theroux
Underwood
Wall Whiteside Whitehead Willowfield Woods
);

my $male_len = scalar(@FORENAMES_MALE);
my $female_len = scalar(@FORENAMES_FEMALE);
my $sur_len = scalar(@SURNAMES);

sub get_prac_name {
    # 1/2 chance of male or female...
    my $sex_chance = int rand(10);
    my $sex = $sex_chance >=5 ? "Male" : "Female";

    my $prac_name = $sex_chance >=5 ? $FORENAMES_MALE[int rand($male_len)] :
         $FORENAMES_FEMALE[int rand($female_len)]; 

    $prac_name .= " $SURNAMES[int rand($sur_len)]"; 
    return $prac_name;
}

say get_prac_name for 1..10;
1;
