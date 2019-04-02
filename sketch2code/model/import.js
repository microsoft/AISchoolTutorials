const fs = require('fs');

const data = JSON.parse(fs.readFileSync(`${__dirname}/dataset.json`, 'utf8'));

console.log(data.length);

data.forEach(row => {
    console.log(row);
})