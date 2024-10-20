import json

def main():
    with open('MasterWordList.json') as fh:
        j = json.loads(fh.read())

    subject_dict = {}
    for entry in j:
        subject_dict[entry['Subject']] = subject_dict.get(entry['Subject'], []) + [entry]

    for subject, entries in subject_dict.items():
        outfile = open(f'{subject}List.json', 'w+')
        outfile.write(json.dumps(entries, indent=1))
        outfile.close()

    

if __name__ == '__main__':
    main()