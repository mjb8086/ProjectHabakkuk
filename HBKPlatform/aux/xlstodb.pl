#!/usr/bin/env perl
# Read XLS into database.
# Copyright 2022 NowDoctor Ltd.

# TODO
# Skip wb 1
# Specialty Names are B,2 onwards
# Interventions/Procedures is C,2 onwards, until we hit a blank
# Conditions/Symptoms is D,2 onwards, until we hit a blank

# !! !! !! Enable this to skip headers in each worksheet !! !! !!
my $SKIP_FIRST_ROW = 1;
my $SKIP_FIRST_COL = 1;

use warnings;
use v5.32;
use Data::Dumper;

# CPAN 
use Spreadsheet::ParseXLSX;

# Global vars
my ($sourceXls, $workbook, $parser);

my $BANNER = << 'EOF';
<source>.clsx will be read and parsed for worksheets. 

Usage:
./xlstodb.pl <source>.xlsx

Eg, ./xlstodb.pl docs.xlsx 
Will produce db tables for each worksheet inside docs.xlsx
EOF


# Begin exec, read args array
die ($BANNER) if (scalar @ARGV != 1);
($sourceXls) = @ARGV;

# Init XLS reader
$parser = Spreadsheet::ParseXLSX->new;
$workbook = $parser->parse($sourceXls);
die "Problems parsing Excel file: " . $parser->error() unless defined $workbook;


# Read each worksheet.

# L A Y O U T
# wsHash - A hash of the worksheet
#   inside wsHash -> hashes of each column named for their column title
#       inside the column hashes -> An array of each row's contents

for my $worksheet ( $workbook->worksheets() ) {
    # Skip fist workbook
    my $wsName = $worksheet->get_name();
    next if $wsName eq "Master";
    my %wsHash;
    my $colTitle;
    my ($rowMin, $rowMax) = $worksheet->row_range();
    my ($colMin, $colMax) = $worksheet->col_range();

    say "Reading worksheet $wsName";
    say "Row range: $rowMin, $rowMax";
    say "Col range: $colMin, $colMax";

    $rowMin = 1 if $SKIP_FIRST_ROW;
    $colMin = 1 if $SKIP_FIRST_COL;

    die "Malformed? We assume 3 columns" if ($colMax != 3);

    for my $col ( $colMin .. 3 ) {
        $colTitle = $worksheet->get_cell(0, $col);
        next if !$colTitle;
        $colTitle = $colTitle->value();
        say "Col title $colTitle";
        $wsHash{$wsName}{$colTitle} = qw();
        for my $row ( $rowMin .. $rowMax ) {
            my $cell = $worksheet->get_cell($row, $col);
            last if !$cell or $cell->value() eq "";
            push @{$wsHash{$wsName}{$colTitle}}, $cell->value();
        }
    }
    print Dumper %wsHash;
}
say "Process complete.";

# Connect to DB and write each hash into an appropriate table.
