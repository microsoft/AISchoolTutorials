from training.train_encoder import EncoderTrainer
from training.train_decoder import DecoderTrainer

if __name__ == '__main__':
    EncTrainer = EncoderTrainer()
    EncTrainer.train()
    print('Finished training encoder')
    EncTrainer.generate_table()
    print('Finished generate_table encoder') 
    DecTrainer = DecoderTrainer()
    DecTrainer.train()
    print('Finished training decoder')
