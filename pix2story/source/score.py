import os, json, base64
import generate 

def init():
    global st_gen
    st_gen = generate.StoryGenerator()

def run(image_rel_path):
    init()
    script_dir = os.path.dirname(__file__)
    image_path = os.path.join(script_dir, image_rel_path)
    prediction = st_gen.story(image_loc=image_path)
    print("Finished!")
    print(prediction)
    return json.dumps({'prediction': prediction})

run("../images/dog.jpeg")
